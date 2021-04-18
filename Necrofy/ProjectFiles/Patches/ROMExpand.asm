lorom

; Function that loads the level pointer from the ROM
org $80886C
ASL A
TAX
JSR loadptr
STA $12

; Functions that read the level data
org $8086A2

load_level:
LDA #$8000
STA $00AA
LDA #$007E
STA $00AC
;LDX $10
;LDA $9F0004,X	; Tilemap
  LDY #$0004
  LDA [$10],Y
STA $00A6
;LDA $9F0006,X	; Tilemap
  INY #2
  LDA [$10],Y
STA $00A8
;LDA $9F0024,X	; Width
;TAY
;LDA $9F0022,X	; Height
;TAX
  LDY #$0022
  LDA [$10],Y
  TAX
  INY #2
  LDA [$10],Y
  TAY
JSL $80ACA2
JSL $80AD2B
;LDX $10
;LDA $9F000A,X	; Collision
;TAY
;LDA $9F0008,X	; Collision
  LDY #$000A
  LDA [$10],Y
  TAX
  DEY #2
  LDA [$10],Y
  TXY
JSL $80AD92
;LDX $10
;LDA $9F000C,X	; Graphics
  LDY #$000C
  LDA [$10],Y
STA $0028
STA $1E80
;LDA $9F000E,X	; Graphics
  INY #2
  LDA [$10],Y
STA $002A
STA $1E82
;LDA $9F0026,X	; Tile priority data upper bound
  LDY #$0026
  LDA [$10],Y
STA $00DC
;LDA $9F0028,X	; Hidden tiles lower bound
  INY #2
  LDA [$10],Y
TAX
LDA #$0000
LDY #$2000
JSL $809F29
;LDX $10
;LDA $9F002C,X	; Player 1 Y start position
  LDY #$002C
  LDA [$10],Y
CLC
;ADC $9F0030,X	; Player 2 Y start position
  LDY #$0030
  ADC [$10],Y
LSR A
SEC
SBC #$0084
;TAY
  PHA
;LDA $9F002A,X	; Player 1 X start position
;CLC
;ADC $9F002E,X	; Player 2 X start position
  DEY #2
  LDA [$10],Y
  LDY #$002A
  ADC [$10],Y
LSR A
SEC
SBC #$0080
TAX
  PLY
JSL $80A9CC
LDA #$7800
STA $1B7E
XBA
ORA #$0001
JSL $80A4D9
;LDX $10
;LDA $9F0010,X	; Pallette data
;TAY
;LDA $9F0012,X	; Pallette data
  LDY #$0010
  LDA [$10],Y
  TAX
  INY #2
  LDA [$10],Y
  TXY
JSL $80A037
;LDX $10
;LDA $9F0014,X	; Sprite pallette data
;TAY
;LDA $9F0016,X	; Sprite pallette data
  LDY #$0014
  LDA [$10],Y
  TAX
  INY #2
  LDA [$10],Y
  TXY
JSL $80A05B
JSL $80A079
;LDX $10
;LDA $9F0018,X	; Palette animation
;ORA $9F001A,X	; Palette animation
  LDY #$0018
  LDA [$10],Y
  INY #2
  ORA [$10],Y
BEQ .no_palette_animation
;LDA $9F001A,X	; Palette animation
;TAY
;LDA $9F0018,X	; Palette animation
  LDA [$10],Y
  TAX
  DEY #2
  LDA [$10],Y
  TXY
JSL $80825E
;LDX $10
.no_palette_animation:
;LDA $9F001C,X	; Monster data
  LDY #$001C
  LDA [$10],Y
STA $00
;LDA #$009F
  LDA $12
STA $02
LDA #$80EC
LDY #$0081
JSL $80825E
;LDX $10
;LDA $9F001E,X	; Victim data
  LDY #$001E
  LDA [$10],Y
STA $00
;LDA #$009F
  LDA $12
STA $02
LDA #$81F6
LDY #$0081
JSL $80825E
;LDX $10
;LDA $9F001E,X	; Victim data
  LDY #$001E
  LDA [$10],Y
TAX
;LDA #$009F
  LDA $12
JSL $82DB46
;LDX $10
;LDA $9F0020,X	; Item data
  LDY #$0020
  LDA [$10],Y
STA $00
;LDA #$009F
  LDA $12
STA $02
LDA #$C8F6
LDY #$0080
JSL $80825E
;CLC
;LDA #$003C
;ADC $10
  LDA #$0040 ; This has been moved back by 4 bytes to insert secret bonus data
PHA

-:
WAI
WAI
WAI
;PLA
;TAX
;CLC
;ADC #$0008
;PHA
;LDA $9F0000,X
;ORA $9F0002,X
  PLY
  PHY
  LDA [$10],Y
  INY #2
  ORA [$10],Y
BEQ .ret
;LDA $9F0004,X
;STA $00
;LDA $9F0006,X
;STA $02
;LDA $9F0002,X
;TAY
;LDA $9F0000,X
  LDA [$10],Y
  TAX
  INY #2
  LDA [$10],Y
  STA $00
  INY #2
  LDA [$10],Y
  STA $02
  INY #2
  PLA
  PHY
  TAY
  LDA [$10],Y
  TXY
JSL $80825E
BRA -

.ret:
PLA
RTL

; Use the space saved here to put the new pointer loading code
loadptr:
LDA $9F8002,X
STA $10
LDA $9F8004,X
RTS

warnpc $808803

org $808803

load_player_pos:
LDA $1E88
BEQ .player2
;LDX $10
;LDA $9F002A,X	; Player 1 x position
  LDY #$002A
  LDA [$10],Y
STA $00
;LDA $9F002C,X	; Player 1 y position
  INY #2
  LDA [$10],Y
STA $02
STZ $04
STZ $06
LDA #$CDF4
LDY #$0080
JSL $808248

.player2:
LDA $1E8A
BEQ .ret
;LDX $10
;LDA $9F002E,X	; Player 2 x position
  LDY #$002E
  LDA [$10],Y
STA $00
;LDA $9F0030,X	; Player 2 y position
  INY #2
  LDA [$10],Y
STA $02
LDA #$0001
STA $04
STA $06
LDA #$CDF4
LDY #$0080
JSL $808248

.ret:
RTL

warnpc $808849

org $808849

load_level_title:
;LDX $10
;LDA $9F0036,X	; Level title 1
;TAY
;LDA $9F0038,X	; Level title 2
;TAX
;LDA $10
  LDY #$0036
  LDA [$10],Y
  PHA
  INY #2
  LDA [$10],Y
  TAX
  PLY
JML $82AC07

warnpc $80885B

org $82AC07

REP #$30
PHB
PEA $0082
PLB
PHX 			; Level title 2
;PHA 			; Level pointer
PHY 			; Level title 1
LDA $136C
ORA #$0080
STA $136C
WAI
JSL $809F9D
JSL $82ACF0
JSL $82AD44
JSL $82ACD6
LDA $136C
AND #$007F
STA $136C
LDA #$000F
STA $136C
LDA #$0001
STA $212C
;LDA #$009F
  LDA $12
PLX 			; Level title 1
JSL $82AD5A
JSR $AE6F
JSL $82AD44
STZ $0016
;PLX            ; Level pointer
;STX $0038
;LDA $9F0032,X  ; Music
  LDY #$0032
  LDA [$10],Y
PHA
;LDA $9F0034,X  ; Sound effects
  INY #2
  LDA [$10],Y
TAX
PLA
JSL $80CBD9
-:
LDA $0016
CMP #$0078
BCC -
;LDA #$009F
  LDA $12
PLX
JSL $82AD5A
JSR $AE6F
STZ $0016
;LDX $0038
;LDA $9F0000,X   ; Tileset
  LDY #$0000
  LDA [$10],Y
PHA
;LDA $9F0002,X   ; Tileset
  INY #2
  LDA [$10],Y

warnpc $82AC87
padbyte $EA ; NOP opcode
pad $82AC87

org $809B14

load_demo:
;LDX $10
;LDA $9F0000,X   ; Tileset
  LDY #$0000
  LDA [$10],Y
PHA
;LDA $9F0002,X   ; Tileset
  INY #2
  LDA [$10],Y
LDX #$007E
LDY #$8000
JSL $80CD20
PLA
JSR $9C72
LDA #$0010
JSL $808353
LDA $6C
JSL $8083D5
JSR $9C4A
JSL $808618
JSL $8088A9
JSL $808632
JSL $8086A2
;LDX $10
;LDA $9F0032,X   ; Music
  LDY #$0032
  LDA [$10],Y
PHA
;LDA $9F0034,X   ; Sound effects
  INY #2
  LDA [$10],Y
TAX
PLA

warnpc $809B5A
padbyte $EA ; NOP opcode
pad $809B5A

org $82C259
load_bonus:
LDA $1E72
STA $005C
LDA $1E74
STA $005E
LDA $1E76
STA $0060
LDA $1E78
STA $0062
;LDX $10
;LDA $9F003A,X
  LDY #$003A
  LDA [$10],Y
BNE .bonuses_exist
STZ $0044
STZ $0046
JSR $C56B
BRA .branch2
.bonuses_exist:
  SEC
  SBC $10
STA $0044
CLC
ADC #$0008
STA $0046
LDA #.part2
STA $1EB2
JSR $C56B
BRA .branch2

.part2:
;LDX $0044
;LDA $9F0000,X
  LDY $0044
  LDA [$10],Y
BEQ .branch3
;INX
;INX
;STX $0044
  INY
  INY
  STY $0044
TAX
JSR ($C346,X)
.branch2:
JSR $C2C6
BCS .branch3
RTS
.branch3:
JSR $D44E
JSR $C2C6
LDY #$0000
LDX #$C369
LDA #$0082
JSL $82B8FB
SEC
RTS

warnpc $82C2C6

; Modify secret bonus code to load from level data instead of a fixed table

org $82D0E0
JSR run_secret_bonus
org $82D104
JSR run_secret_bonus
org $82D11D
JSR get_bonus_level

org $82D17E
run_secret_bonus:
LDY #$003C
LDA [$10],Y
DEC A
PHA
RTS

get_bonus_level:
LDY #$003E
LDA [$10],Y
RTS

warnpc $82D25E

; Modify password code to use a set string for each password

org $82B02D
; Code for checking an entered password
PHD
PEA $0000
PLD
LDA $82B14A
STA $38
LDA $82B14C
STA $3A

LDX #$0001 ; victim count
LDY #$0000 ; password data index
LDA #$0005
STA $1E7C ; level
BRA +

-:
	INY
	INY
	INX
	CPX #$000B
	BNE +
	
	LDX #$0001
	LDA $1E7C
	CLC
	ADC #$0004
	CMP $82B14E
	BEQ no_password_found
	STA $1E7C
+:
	LDA [$38],Y
	INY
	INY
	CMP $1EA0
	BNE -

	LDA [$38],Y
	CMP $1EA2
BNE -

; password found
CPX #$000A
BNE +
	LDX #$0010
+:
STX $1D52
STX $1D50
PLD
CLC
RTL

no_password_found:
LDA #$0001
STA $1E7C
PLD
SEC
RTL

warnpc $82B0DE

org $82B0DE
; Code for getting the password to show
PHD
PEA $0000
PLD
LDA $82B14A
STA $38
LDA $82B14C
STA $3A

; Get level index * 10
LDA $1E7C
LSR A
DEC A
DEC A
STA $3C
ASL A
ASL A
CLC
ADC $3C
STA $3C

; Get total victims saved
SED
LDA $1F9C
CLC
ADC $1F9E
SEC
SBC #$0001
CLD

; Add together to get the final index
CLC
ADC $3C
ASL A
ASL A

TAY
LDA [$38],Y
STA $1EA0
INY
INY
LDA [$38],Y
STA $1EA2
STZ $1EA4
PLD
RTL

warnpc $82B14A
