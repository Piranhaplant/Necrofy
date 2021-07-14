; Fixes the pop-up wall tiles from turning into the wrong tile when placed in a level directly then shot with a bazooka.
lorom

org $81ECAC
PHA
PHD

LDA $0C
LSR A
LSR A
LSR A
LSR A
LSR A
LSR A
TAX ; Use X and Y registers to hold these values to save some bytes

LDA $10
LSR A
LSR A
LSR A
LSR A
LSR A
LSR A
TAY

LDA #$0000
TCD

PHX
PHY
JSL $80ACF6
LDA [$28] ; Don't use indexing to save more bytes

; Increment tile number by one first if it is odd
BIT #$0001
BEQ +
	INC A
+:
EOR #$0001

PLY
PLX
JSL $80AB5A

warnpc $81ECE0
padbyte $EA ; NOP opcode
pad $81ECE0
