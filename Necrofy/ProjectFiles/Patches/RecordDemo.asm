lorom

!DEMO_LENGTH = $1EBE
!PREVIOUS_INPUT = $1EB8
!PREVIOUS_INPUT_COUNT = $1EC0

org $80FFD8
db 5 ; Set SRAM size to 0x8000 bytes

; Skip to demo on startup
org $80913C
LDA #$0000
STA $700000
; Stuff copied from the end of $80:9847
JSL $82AD44
JSR $9A31
JSR $9C52

JSR $9AB0		; Run the demo

; Setup demo
org $809B0A
LDA.w #!DEMO_LEVEL

; Every frame during the demo
org $809CB2
INC $1EC2 ; Increment frame counter
LDA $1EC2 ;\ Copy to game's RNG counter
STA $0024 ;/
LDA $4218 ; Joypad 1 Data
STA $6E
XBA
AND #$000F
TAY
LDA $9D0C,Y
AND #$00FF
STA $72

LDA $6E
CMP !PREVIOUS_INPUT
BEQ +
	LDA !PREVIOUS_INPUT_COUNT
	BEQ + ; Don't write the initial 0 input to the data if it was used for 0 frames
	
	LDX !DEMO_LENGTH
	LDA !PREVIOUS_INPUT
	STA $700002,X
	LDA !PREVIOUS_INPUT_COUNT
	STA $700004,X
	
	TXA
	CLC
	ADC #$0004
	STA !DEMO_LENGTH
	
	LDA $6E
	STA !PREVIOUS_INPUT
	STZ !PREVIOUS_INPUT_COUNT
+:
INC !PREVIOUS_INPUT_COUNT

SEC
RTL

warnpc $809D0C

; After the demo is done
org $809B8B
BCC +
	LDA #$0090
	JSL $808353		; Wait 0x90 frames
	JSL $80CC27		; Stop music
	JSL $808933		; Fade screen out
+:

LDX !DEMO_LENGTH
LDA !PREVIOUS_INPUT
STA $700002,X
LDA !PREVIOUS_INPUT_COUNT
STA $700004,X
TXA
CLC
ADC #$0004
STA $700000

JSL $8091F7 ; Show copyright screen

warnpc $809BCF

; Wait forever after copyright screen shows
org $809242
-:
BRA -

; Replace copyright screen tilemap
org $9EDE80
incbin RecordingComplete.tlm
