; Adds several debug commands to the pause screen

!prev_input = $0040
!controller_num = $0042
!new_inputs = $0044
!cursor_position = $0046 ; Premultiplied by 4
!menu_data_size = $0048 ; Premultiplied by 4

!kill_victim_sprite_type = #$0040

org $8089B0

LDX #$0000
LDA $006E
BIT #$1000
BNE paused

LDX #$0002
LDA $0070
BIT #$1000
BNE paused
RTL

paused:
LDA $006E,X
STA !prev_input
STX !controller_num

LDA #$0040
JSL $80CC5D     ; Decrease volume
JSL debug_menu
LDA #$007F
JSL $80CC5D     ; Normal volume
RTL

warnpc $808A00

freecode $33
debug_menu:
PHB
PHK
PLB

; Setup
STZ !cursor_position
LDX #$0000
-:
	LDA.w menu_options,X
	BEQ +
	INX #4
BRA -
+:
STX !menu_data_size

; Clear HUDs
LDA #$0000
JSL $80C1CF
LDA #$0002
JSL $80C1CF
JSL $80C07F

; Turn off victim radars
LDA $1F98
ORA $1F9A
BEQ +
	STZ $1F98
	STZ $1F9A
	LDA #$0003
	JSL $808353
+:

LDA.w #draw_menu
LDY.w #draw_menu>>16
JSL $8083AE ; Execute on VBlank
; Enable window to make the background darkened
LDA #$FF00
STA $2126

-:
	WAI
	; Calculate new button presses
	LDX !controller_num
	LDA $006E,X
	PHA
	STA !new_inputs
	EOR !prev_input
	AND !new_inputs
	STA !new_inputs
	PLA
	STA !prev_input
	
	LDA !new_inputs
	BIT #$1000
	BNE end_pause
	BIT #$0080
	BNE a_button
	BIT #$0800
	BNE up_button
	BIT #$0400
	BNE down_button
	BRA -
a_button:
	LDX !cursor_position
	INX #2
	JSR (menu_options,X)
	BRA end_pause
up_button:
	LDY #$0000
	JSR draw_cursor
	LDA !cursor_position
	BNE +
		LDA !menu_data_size
	+:
	DEC #4
	BRA cursor_moved
down_button:
	LDY #$0000
	JSR draw_cursor
	LDA !cursor_position
	INC #4
	CMP !menu_data_size
	BNE +
		LDA #$0000
	+:
cursor_moved:
	STA !cursor_position
	LDY #$3CB1
	JSR draw_cursor
BRA -

end_pause:
; Wait until start and A buttons aren't held anymore
LDX !controller_num
-:
	LDA $006E,X
	BIT #$1080
BNE -

; Refresh entire HUD
JSL $80C2F7
; Disable window
LDA #$0000
STA $2126

LDA.w #clear_tilemap
LDY.w #clear_tilemap>>16
JSL $8083AE ; Execute on VBlank

PLB
RTL

draw_cursor:
LDA !cursor_position
ASL A
ASL A
ASL A
CLC
ADC #$6461
STA $2116
STY $2118
RTS

draw_menu:
PHB
PHK
PLB

; Set VRAM increment to word mode
SEP #$20
LDA #$80
STA $2115
REP #$20

LDA #$6423
STA $2116
LDY.w #text_debug
JSR draw_string
	
LDX #$0000
-:
	LDA.w menu_options,X
	BEQ .exit
	TAY
	
	; Calculate VRAM write address
	TXA
	ASL A
	ASL A
	ASL A
	CLC
	ADC #$6463
	STA $2116
	
	JSR draw_string
	INX #4
BRA -

.exit:
LDY #$3CB1
JSR draw_cursor

PLB
RTL

draw_string:
-:
	LDA $0000,Y
	AND #$00FF
	BEQ .exit
	; Convert character to tile
	SEC
	SBC #$0020
	CMP #$0010
	BCC +
		SEC
		SBC #$0009
	+:
	CMP #$0018
	BCC +
		CLC
		ADC #$0090-$0018
	+:
	; Calculate tile number/properties and store
	CLC
	ADC #$3C00
	STA $2118
	INY
BRA -
.exit:
RTS

clear_tilemap:
LDA #$6420
STA $2116
LDA !menu_data_size
CLC
ADC #$0008
ASL A
ASL A
ASL A
TAX
-:
	STZ $2118
	DEX
BPL -
RTL

text_debug:
db "DEBUG",$00

menu_options:
dw text_get_10000_points,get_10000_points
dw text_give_secret_bonus,give_secret_bonus
dw text_save_victims,save_victims
dw text_kill_victims,kill_victims
dw text_kill_player,kill_player
dw $0000

text_get_10000_points:
db "GET 10000 POINTS",$00
get_10000_points:
LDA !controller_num
ASL A
TAX
LDA $1E74,X
SED
CLC
ADC #$0001
CLD
STA $1E74,X
RTS

text_give_secret_bonus:
db "GIVE SECRET BONUS",$00
give_secret_bonus:
LDX !controller_num
INC $1FF0,X
RTS

text_save_victims:
db "SAVE ALL VICTIMS",$00
save_victims:
LDA $1D52 ; Number of victims left on level
BEQ .exit

LDA $1D50 ; Number of victims on level start
SED
SEC
SBC #$0001
CLD
TAX
-:
	PHX
	
	LDA $7E605A,X ; Victim status
	AND #$00FF ; Check if victim is currently spawned
	BEQ .inactive_victim
	CMP #$0001
	BNE +
	.spawned_victim:
		; Get entity index
		LDA $7E609A,X
		AND #$00FF
		PHA
		; Calculate sprite type for current player
		LDX !controller_num
		LDA $80FE64,X
		TAY
		
		PLX
		JSL $808480 ; Perform collision
		BRA +
	.inactive_victim:
		TXA
		JSL $818191 ; Set as ignored
		JSL $80C863 ; Decrement number of victims left on level
		
		LDY #$0010 ; Victim type (cheerleader)
		LDX !controller_num
		LDA $1E84,X ; Get character for current player
		BEQ ++
			LDA #$8000 ; Set high bit for Julie
		++:
		JSL $80C81F ; Add saved victim
	+:
	
	PLX
	DEX
BPL -

LDA #$0009
JSL $80CC3B ; Play sound effect

.exit:
RTS

text_kill_victims:
db "KILL ALL VICTIMS",$00
kill_victims:
LDA $1D52 ; Number of victims left on level
BEQ .exit

LDA $1D50 ; Number of victims on level start
SED
SEC
SBC #$0001
CLD
TAX
-:
	PHX
	
	LDA $7E605A,X ; Victim status
	AND #$00FF ; Check if victim is currently spawned
	BEQ .inactive_victim
	CMP #$0001
	BNE +
	.spawned_victim:
		LDA $7E609A,X ; Get entity index
		AND #$00FF
		TAX
		LDY !kill_victim_sprite_type
		JSL $808480 ; Perform collision
		BRA +
	.inactive_victim:
		TXA
		JSL $818191 ; Set as ignored
		JSL $80C863 ; Decrement number of victims left on level
	+:
	
	PLX
	DEX
BPL -

LDA #$0024
JSL $80CC3B ; Play death sound effect

.exit:
RTS

text_kill_player:
db "KILL PLAYER",$00
kill_player:
LDX !controller_num
STZ $1CB8,X ; Set health to 0
STZ $1D4C,X ; Set lives to 0
RTS

freecode $33

victim_collision:
LDX $1E
BNE .already_collided

; Add a check for insta-kill sprite type
CMP !kill_victim_sprite_type
BNE +
	STZ $26 ; Clear victim health
	LDA #$0003 ; Set collision type to monster
+:
RTL

.already_collided:
PLA ; \
PLB ; / Ignore return to other subroutine
SEC
RTL

; Override victim collision
org $83A364
JSL victim_collision

; Modify cheerleader to be able to be insta-killed even when jumping
org $839F13
BNE cheerleader_collision_triggered
org $839F30
BNE cheerleader_collision_triggered
org $839F41
cheerleader_collision_triggered:
