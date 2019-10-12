lorom

; Remove original code that sets default weapons/specials
org $808891
RTL

; Overwrite code used for the intro and menus
org $809126
LDA #$0001
STA $1E88 ; Player 1 is in game flag
LDA #$0000
STA $1E84 ; Which character player 1 is
LDA.w #!LEVEL
STA $1E7C
LDA.w #!VICTIMS
STA $1D52 ; Victims left on level
STA $1D50 ; Victims on level start

LDX #weapon_table
LDY #$1CCC
LDA.w #!WEAPON_COUNT*2-1
MVN $7E,$80

LDX #special_table
LDY #$1D0C
LDA.w #!SPECIAL_COUNT*2-1
MVN $7E,$80
RTL

weapon_table:
!i = 0
while !i < !WEAPON_COUNT
	dw !{WEAPON!{i}}
    !i #= !i+1
endif

special_table:
!i = 0
while !i < !SPECIAL_COUNT
	dw !{SPECIAL!{i}}
    !i #= !i+1
endif

warnpc $80918E
