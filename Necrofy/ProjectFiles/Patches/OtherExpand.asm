; Contains modifications for adding extra sprite graphics, extra sprite tile data, and
; larger compressed data

lorom

; Allow more banks to be used for sprite tile data
org $80BD9C
CMP #$00B0

; There was a table here, but it has been obsoleted by modifying some code below
org $80B447

set_graphics_base:
PHX
PHY
LDX #$0084
LDY #$8000
CMP.w #!extra_sprite_graphics_start_index
BCC + ; Branch if less than
LDX.w #!extra_sprite_graphics_base>>16
LDY.w #!extra_sprite_graphics_base
SEC
SBC.w #!extra_sprite_graphics_start_index
+:
STX $80
STY $7E
PLY
PLX
RTS

read_byte:
LDA $38			; Number of bytes remaining
BEQ +
DEC $38
LDA [$28]		; Load current byte
AND #$00FF
INC $28
CLC
RTS
+:
LDA $42			; If there is additional compressed data
BEQ +
LDA [$28]		; Get additional data pointer low bytes
TAX
INC $28
INC $28
LDA [$28]		; Get additional data pointer bank
STX $28
STA $2A
JSR load_size
BRA read_byte
+:
SEC
RTS

next_sprite_tile:
; New code
LDA $86
CMP #$0080
BNE +
LDA [$8A]
TAY
INC $8A
INC $8A
LDA [$8A]
STY $8A
STA $8C
LDA [$8A]
AND #$00FF
STA $86
INC $8A
+:
; Code from original subroutine
LDY #$0002
RTS

enable_hdma:
; Fix setting the HDMA enable byte with the accumulator set to 16-bit, thus clearing the FastROM enable flag
SEP #$20
LDA $136E
STA $420C
REP #$20
CLC
RTL

warnpc $80B547



org $82DC45
LDA.w #enable_hdma
LDY.w #enable_hdma>>16
JML $8083AE

org $80B9EC
find_vram_tile_index:
STX $3A			; Backup sprite tile number * 2
LDX $9E			; Current VRAM tile index
LDA $A0			; Copy of global frame counter
; Find VRAM tile index that has not been used on the current frame
; This loop has been rearranged to avoid needing the table at $80B447
-:
INX
INX
CPX #$0100
BNE +
LDX #$0000
+:
CMP $175E,X		; Frame for VRAM tile index
BEQ -

STX $9E			; Update current VRAM tile index
STA $175E,X		; Store frame that tile was used on
LDA $7E4128,X	; Sprite tile number used by VRAM tile index
BMI +			; If there was nothing previously there, skip ahead
TAX				; \
LDA #$FFFF		; | Clear the VRAM tile used by the old sprite tile
STA $7E2128,X	; /
+:
LDA $9E			; Current VRAM tile index
LDX $3A			; Sprite tile number * 2
STA $7E2128,X	; Store VRAM tile index for sprite tile number
TAX
LDA $3A			; Sprite tile number * 2
STA $7E4128,X	; Store sprite tile number for VRAM tile index
LSR A			; Get back actual sprite tile number
JSR set_graphics_base
XBA				; \ Multiply the sprite tile number by 0x80
PHA				; |
LSR A			; |
ADC $7E			; | Add the base address for sprite graphics
AND #$FF80		; /
LDY $7C			; Current index into tables for graphics to load
STA $15DE,Y		; Store low bytes of graphics to load
PLA
AND #$00FF
CLC
ADC $80			; Add the bank of sprite graphics base address
STA $165E,Y		; Store bank of graphics to load
LDA $B547,X		; Get destination address for this VRAM tile index
STA $16DE,Y		; Store destination address
INY
INY
STY $7C			; Increment index into graphics to load tables
LDA $B647,X		; Get actual tile number for VRAM tile index
LDX $38			; Restore OAM table index
RTS

warnpc $80BA51

org $80BA53
JSR next_sprite_tile
org $80BABC
JSR next_sprite_tile
org $80BB32
JSR next_sprite_tile
org $80BBA8
JSR next_sprite_tile

org $80CD20

decompress:
PHD
PEA $0000
PLD
STA $2A			; Source address bank
LDA $06,S		; Source address low bytes
STA $28
STX $2E			; Destination address bank
STY $2C			; Destination address low bytes
STY $40			; Will be used at the end to calculate number of decompressed bytes
PHB
LDA #$2020		; \
STA $7E6F00		; |
LDA #$0FED		; | Fill (most of) dictionary with 0x20 bytes
LDX #$6F00		; |
LDY #$6F01		; |
MVN $7E,$7E		; /
PLB
JSR load_size
LDA #$0FEE		; Dictionary write position
STA $3A
LDY #$0000		; Number of bits left in format bit
.loop:
TYX
BNE +
LDY #$0008
JSR read_byte	; Get format byte into A
BCS .ret		; Exit if no bytes left
+:
DEY				; Decrement number of bits left in format bit
LSR A			; Shift format byte
BCC +			; If bit was 0
PHA
JSR read_byte	; Get next byte
BCS .ret2		; Exit if no bytes left
JSR write_byte	; Write byte into output
LDX $3A			; \
SEP #$20		; |
STA $7E6F00,X	; |
REP #$20		; | Write byte into dictionary
INX				; |
TXA				; |
AND #$0FFF		; |
STA $3A			; /
PLA
BRA .loop		; Loop back
+:
PHA
PHY
JSR read_byte	; Get next byte
BCS .ret3		; Exit if no bytes left
STA $3C
JSR read_byte	; Get next byte
BCS .ret3		; Exit if no bytes left
TAY
ASL A			; \
ASL A			; |
ASL A			; |
ASL A			; | Combine with the upper 4 bits of the previous byte
AND #$0F00		; | This will be the position to read from the dictionary
ORA $3C			; |
STA $3C			; /
TYA				; \
AND #$000F		; | Get the lower 4 bits + 2
INC A			; | This will be the number of bytes to read
INC A			; | Although actually one more byte than this will be copied
STA $3E			; /
LDX $3C			; Dictionary read position
LDY $3A			; Dictionary write position
-:
SEP #$20		; 8-bit A
LDA $7E6F00,X	; Get byte from dictionary
PHX				; \
TYX				; | Write byte into dictionary
STA $7E6F00,X	; |
PLX				; /
REP #$20		; 16-bit A
JSR write_byte	; Write byte into output
INY				; \
TYA				; | Increment dictionary write position
AND #$0FFF		; |
TAY				; /
INX				; \
TXA				; | Increment dictionary read position
AND #$0FFF		; |
TAX				; /
DEC $3E			; Decrement number of bytes to read
BPL -			; Loop while there are still more bytes

STY $3A
PLY
PLA
BRA .loop		; Loop back
.ret3:
PLA
.ret2:
PLA
.ret:
SEC				; \
LDA $2C			; | Calculate total number of bytes written
SBC $40			; |
TAY				; /
PLD
RTL

load_size:
STZ $42			; Default to no extra data
LDA [$28]		; Number of compressed bytes (not including these 2)
BPL +			; Skip setting the extra data flag if the high bit was not set
AND #$7FFF		; Remove high bit
INC $42			; Set extra data flag
+:
STA $38
INC $28
INC $28
RTS

write_byte:
SEP #$20		; 8-bit A
STA [$2C]		; Write output byte
REP #$20		; 16-bit A
INC $2C
RTS

warnpc $80CDF4
