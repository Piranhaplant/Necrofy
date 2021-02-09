; Allows the players to cycle their weapons and items in reverse order by holding the L button. The R and Select buttons will open the victim radar.
org $80EA63

next_weapon:
LDX #$0000
LDA #$000F
JSR find_next
JSR $EA4B		; Setup weapon shooting sprite
RTS

find_next:
STA $2E			; Store counter

ASL A			; \
DEC #2			; | Store max index value
STA $0038		; /

LDA $64,x		; \
STA $2C			; / Store quantity table RAM address

TXA				; \
ASL A			; |
CLC				; | Get current weapon/item
ADC $0E			; |
TAX				; |
LDA $1CBC,x		; /

BPL +			; 0xFFFF indicates no weapon/item
	LDA #$0000
+:
ASL A			; \
TAY				; / Convert for indexing
BRA start_loop

done:
PHA
CMP $1CBC,x
BEQ +
	STA $1CBC,x		; Store new weapon
	LDA #$0012
	JSL $80CC3B		; Play sound effect
+:
PLA				; The new weapon value needs to be in the accumulator for $EA4B to setup the sprites
RTS

warnpc $80EAA8

org $80EAA8

next_special_item:
LDX #$0002
LDA #$000D
JSR find_next
RTS

; Put the main loop of find_next in a separate location so that both of
; the existing subroutines can start at their original locations
-:
	LDA ($2C),y		; Get quantity
	BNE weapon_found
	start_loop:
	LDA $1A			; Controller input
	AND #$0020		; L button
	BNE reversed
		INY				; \
		INY				; / Move to next
		CPY $0038		; \
		BNE +			; | Loop back to 0 at max value
			LDY #$0000	; /
		+:
		BRA ++
	reversed:
		CPY #$0000		; \
		BNE +			; | Loop back to max value at 0
			LDY $0038	; /
		+:
		DEY				; \
		DEY				; / Move to previous
	++:
	DEC $2E			; \
BNE -				; / Loop while there are more weapons to check
; No weapon found
	LDA #$FFFF		; Special ID used to indicate none selected
	BRA done
weapon_found:
	TYA				; \
	LSR A			; / Convert index back into weapon number
	BRA done

warnpc $80EAE1

; Change victim radar to R or Select
org $80D290
AND #$2010
org $80D297
AND #$2010
