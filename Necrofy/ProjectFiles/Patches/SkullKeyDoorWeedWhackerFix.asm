; Fixes the bug that allows players to break skull key doors from above in the grass and castle tilesets by using a weed whacker on them. 
; Note: The doors in the castle tileset can still be punched through with a monster potion unless the collision of tile 0x1D3 is updated.
lorom

org $81E98B
-:

org $81E9C3
LDX #$0000
BIT #$0001
BNE -
BIT #$4000
BNE +
INX
INX
BIT #$8000
BEQ -
+:
PHY
LDA $EA04,X
NOP
warnpc $81E9DC

org $81EA04
dw $1897,$11DB
