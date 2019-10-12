lorom

!VICTIMS = $10

!WEAPON0 = $100
!WEAPON1 = $1
!WEAPON2 = $2
!WEAPON3 = $3
!WEAPON4 = $4
!WEAPON5 = $5
!WEAPON6 = $6
!WEAPON7 = $7
!WEAPON8 = $8
!WEAPON9 = $9
!WEAPON10 = $10
!WEAPON11 = $11
!WEAPON12 = $12
!WEAPON13 = $13
!WEAPON_COUNT = 14

!SPECIAL0 = $99
!SPECIAL1 = $1
!SPECIAL2 = $2
!SPECIAL3 = $3
!SPECIAL4 = $4
!SPECIAL5 = $5
!SPECIAL6 = $6
!SPECIAL7 = $7
!SPECIAL8 = $8
!SPECIAL9 = $9
!SPECIAL10 = $10
!SPECIAL11 = $11
!SPECIAL_COUNT = 12

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
