; Fixes the perfect player bonus to correctly give player 2 points.
org $82D44E

STZ $1E94
-:
LDA $1E94
TAX
ASL A
TAY
LDA $1E74,Y
CMP $7E6054,X
BCC next_player2 ; Change jump location to save bytes
LDA $1E88,X
BEQ next_player2 ; Change jump location to save bytes
LDX $1E94
SED
LDA $7E6054,X
CLC
ADC #$0004
STA $7E6054,X
CLD
LDA $1F9C
CLC
ADC $1F9E
CMP #$000A
BCS +
TXY
LDX $D570,Y
LDA #$0082
LDY #$0000
JSR $D774
LDX $1E94
LDA $1F9C,X
INC A
STA $1F9C,X
SED
LDA #$0001
CLC
ADC $1D50
STA $1D50
LDA #$0001
CLC
ADC $1D52
STA $1D52
CLD
next_player2:
BRA next_player ; Change from JMP to save a byte
+:
LDX $1E94
LDA $1D4C,X
CMP #$0005
BCS +
INC A
STA $1D4C,X
JSR $D4E9
BRA next_player
+:
LDY $1E94
LDX $D6C8,Y
LDA values,Y ; Use different value for players 1 and 2 to fix the bug
LDY #$5000
JSR $D774
next_player:
LDA $1E94
INC A
INC A
STA $1E94
CMP #$0004
BCS +
JMP -
+:
RTS

values:
dw $0082,$0482

print pc
warnpc $82D4E9
