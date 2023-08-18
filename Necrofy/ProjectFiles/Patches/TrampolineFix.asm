; Fixes the following bugs when jumping off a trampoline:
; - Landing on a conveyor belt is treated as another trampoline
; - Getting stuck when landing near a solid tile (these jumps are no longer allowed)
; - Being able to jump outside the bounds of the level

org $80E148
	autoclean JSL check_jump
	BCS +
		; Tile can't be jumped to
		JMP $E090
	+:
		; Tile can be jumped to
		AND #$FFFE ; Remove the solid bit from the collision type
		STA $5C
		LDA #$0004
		JMP $F70E
warnpc $80E180

org $80E1D4
	CMP #$0008 ; Make sure only the trampoline middle bit is set
	BNE +
org $80E1EA
	+:

freecode $33
!collision = $0038
!x_position = $003A
!y_position = $003C
check_jump:
	STX !x_position
	STY !y_position
	; Check if position is outside the level
	JSL $80B422
	BCS .ret_false
	; Check if short jump would lead to another trampoline
	LDX !x_position
	LDY !y_position
	JSL $80ADC8 ; Get background collision
	AND #$FFFE ; Remove the solid bit
	CMP #$0008 ; Make sure only the trampoline center bit is set
	BEQ .ret_true
	; Move index to a long jump
	LDX $5A
	INX
	INX
	INX
	INX
	STX $5A
	; Calculate position for long jump
	LDA $80E27D+2,x
	CLC
	ADC $32
	TAY
	STY !y_position
	LDA $80E27D,x
	CLC
	ADC $30
	TAX
	STX !x_position
	; Check if position is outside the level
	JSL $80B422
	BCS .ret_false
	; Check if long jump would lead to another trampoline
	LDX !x_position
	LDY !y_position
	JSL $80ADC8 ; Get background collision
	STA !collision
	AND #$FFFE ; Remove the solid bit
	CMP #$0008 ; Make sure only the trampoline center bit is set
	BEQ .ret_true
	; Check if jump would lead to water
	LDA !collision
	AND #$0101
	CMP #$0101
	BEQ .ret_true
	; Check if tile and surrounding tiles are all not solid
	LDX.w #20
	-:
		PHX
		
		LDA !y_position
		CLC
		ADC.l tile_offsets+2,x
		TAY
		LDA !x_position
		CLC
		ADC.l tile_offsets,x
		TAX
		
		JSL $80ADC8 ; Get background collision
		PLX
		BIT #$0001
		BNE .ret_false
		
		DEX #4
	BPL -

.ret_true
	SEC
	RTL

.ret_false
	CLC
	RTL

tile_offsets:
	dw 0,0   ; The tile itself
	dw -8,0  ; The left tile
	dw -8,-8 ; The above-left tile
	dw 0,-8  ; The above tile
	dw 8,-8  ; The above-right tile
	dw 8,0   ; The right tile
