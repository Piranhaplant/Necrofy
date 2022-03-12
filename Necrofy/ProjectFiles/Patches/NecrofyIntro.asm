; Adds a screen that says "Made with Necrofy" after the LucasArts screen.
; Pixel art logo by Droter.
!graphics_file = NecrofyGraphics.gfx
!tilemap_file = NecrofyTilemap.tlm
!palette_file = NecrofyPalette.plt

org $80914E
JSL necrofy_screen

freecode $33
necrofy_screen:
JSL $809FB0
JSL load_gfx
; Load palette
LDA.w #palette>>16
LDX.w #palette
LDY.w #filesize("!palette_file")
JSL $80C872

LDA #$0002
STA $212C		; Enable BG 2
STZ $1364		; BG 2 horizontal scroll offset
STZ $1366		; BG 2 vertical scroll offset
; Load tilemap
LDA.w #tilemap>>16
PHA
LDA.w #tilemap
LDX #$6400
LDY.w #filesize("!tilemap_file")
JSL $80C8B8

PLA
JSR fade_from_black
-:
	LDA $136C	; Screen display register
	CMP #$000F
BNE -			; Wait for screen to be full brightness
LDA #$00C0
JSL $808353		; Wait 0xC0 frames
JSR fade_to_black
-:
	LDA $136C	; Screen display register
	AND #$0080
BEQ -			; Wait for screen to be black
WAI
SEP #$20		; 8-bit A
STZ $212C		; Disable all layers
REP #$20		; 16-bit A
JML $8091F7 	; Go to the original trademark screen

fade_from_black:
LDA #$0000
STA $136C
LDA #$9C63
LDY #$0080
JSL $8083AE
RTS

fade_to_black:
LDA #$9C7D
LDY #$0080
JSL $8083AE
RTS

load_gfx:
JSL $80C2F7
LDA.w #.vblank
LDY.w #.vblank>>16
JSL $8083AE
SEP #$20
LDA #$64
STA $2108
LDA #$44
STA $210B
REP #$20
RTL

.vblank:
LDA #$1801
STA $4300
LDA.w #graphics
STA $4302
LDA.w #graphics>>16
STA $4304
LDA.w #filesize("!graphics_file")
STA $4305
LDA #$4000
STA $2116
SEP #$20
LDA #$80
STA $2115
LDA #$01
STA $420B
REP #$20
REP #$21
RTL

freedata $33
graphics:
incbin !graphics_file

freedata $33
tilemap:
incbin !tilemap_file

freedata $33
palette:
incbin !palette_file
