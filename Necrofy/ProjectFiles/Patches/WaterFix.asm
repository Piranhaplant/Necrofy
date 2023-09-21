; Fixes the player being able to go off screen when jumping into water

; Fix going into water from bazooka knockback
org $80DEDE
	BCC bazooka_not_solid

org $80DF05
	LDX $60
	LDY $62
	JSL $80B422	; Check if position is outside the level
	BCS exit_bazooka
	JMP $DD0D ; Jump into water
bazooka_not_solid:
	LDX $34
	LDY $36
	JSL $80A8B3	; Check if position collides with edge of screen in multiplayer
	BCS exit_bazooka

org $80DF45
exit_bazooka:

; Fix normal jumping into water
org $80E7D0
	JSL check_water_jump

freecode $33
check_water_jump:
	JSL $80A8B3	; Check if position collides with edge of screen in multiplayer
	BCS .ret
	
	LDX $60
	LDY $62
	JSL $80B422	; Check if position is outside the level
.ret:
	RTL
