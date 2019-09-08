;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;                                                                            ;;
;;  ZAMN Level Expansion ASM Hack                                             ;;
;;                                                                            ;;
;;  This converts all level loading code in the game to utilize 4 byte        ;;
;;      pointers, allowing levels to be placed anywhere in the ROM            ;;
;;  To be assembled an inserted with xkas v0.06 by byuu                       ;;
;;  Created for Necrofy https://github.com/Piranhaplant/Necrofy               ;;
;;                                                         by Piranhaplant    ;;
;;                                                                            ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Other notes:
; I have not noticed any bugs while using this hack except the demo playthroughs
; often desync. I imagine this would be fixed if new demos were recorded on
; the hacked ROM though.

lorom

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  Custom functions                                                          ;;
;;  These functions are inserted in free space in the $80/$82 bank            ;;
;;  Used because not all of the new code can fit in the original space        ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; This is a custom function that will read the pointers as 4 bytes
org $80FF70 ; the freespace location of the function

loadptr:         ; Function that loads the 4 byte pointer from the ROM
; Accumulator contains the level number that is indexed into the pointer table
; A has already been doubled to index 2 byte pointers before this is called
ASL A            ; Double A again to make it work for 4 bytes
TAX              ; Copy for indexing
LDA $9F8002,X    ; Load the first 2 bytes from of the level pointer
STA $10          ; Store it to the same address used by the original function
LDA $9F8004,X    ; Load the second 2 bytes (this is the bank)
STA $12          ; Store it to a custom address (I don't think $12 is used)
RTS              ; Return
; This second function will load the bank from $12
; There are four versions, one that loads to X, one that doesn't,
; and two that are used to save a few bytes elsewhere
loadbank80_4:
JSR $8086A2
BRA loadbank80
loadbank80_3:
STZ $04          ; Moved from original function to save space there
STZ $06
loadbank80:
LDX $10          ; Load the original level pointer (put here to save space)
loadbank80_2:
LDA $12          ; Load the custom bank
XBA              ; Switch the bytes so the actual bank will be pulled last
PHA              ; Put on stack so it can be moved into the bank
PLB              ; Replace the bank with the new value
PLB              ; It's done twice because PHA will push 2 bytes
RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

org $82F980 ; There's a huge chunk of freespace right here

; Here's the $82 bank version of the bank loading helper function
; 3 Versions. 2 and 3 are used for saving bytes in the bonus loading
loadbank82_3:
PHB
JSR loadbank82
LDX $0044
LDA $0000,X
PLB
RTS

loadbank82_2:
LDA $1E78
STA $0062

loadbank82:
LDX $10
LDA $12
XBA
PHA
PLB
PLB
RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  Overridden functions                                                      ;;
;;  I have to get those custom ones in the game somehow                       ;;
;;  All this code replaces stuff in the original game                         ;;
;;  Much of it is actually copied from the game and modified                  ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; The original function that loads the level pointer from the ROM
org $80886C

JSR loadptr    ; Make it jump to my custom function
NOP            ; Dummy out the old stuff that is replaced by the new function
NOP
NOP
NOP

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; This is the main function that reads a lot of the level data
; The entire function has to be copied because so much stuff is moved around
; Lines prefixed with c80XXXX are copied from the original function
; The +X or -X keep track of how many bytes ahead or behind the new function is,
;   because I need it to fit in the original space.
org $8086A2

c8086A2: LDA #$8000
c8086A5: STA $00AA
c8086A8: LDA #$007E
c8086AB: STA $00AC
;8086AE: LDX $10           ; LDX $10 done in loadbank80 ; +2
         PHB               ; Backup the bank     ; +1
         JSR loadbank80    ; Load the new bank   ; -2
c8086B0: LDA $0004,X       ; Tile data pointer   ; -1
c8086B4: STA $00A6
c8086B7: LDA $0006,X       ; Tile data pointer   ;  0
c8086BB: STA $00A8
c8086BE: LDA $0024,X       ; Width               ; +1
c8086C2: TAY
c8086C3: LDA $0022,X       ; Height              ; +2
c8086C7: TAX
         PLB               ; Restore bank        ; +1
c8086C8: JSR $80ACA2
c8086CC: JSR $80AD2B
         PHB
         JSR loadbank80                          ; -3
c8086D2: LDA $000A,X       ; Map 16              ; -2
c8086D6: TAY
;8086D0: LDX $10                                 ;  0
c8086D7: LDA $0008,X       ; Map 16              ; +1
         PLB                                     ;  0
c8086DB: JSR $80AD92       ; Load Map 16
         PHB
         JSR loadbank80                          ; -4
;8086DF: LDX $10                                 ; -2
c8086E1: LDA $000C,X       ; Graphics            ; -1
c8086E5: STA $0028
c8086E8: STA $1E80
c8086EB: LDA $000E,X       ; Graphics            ;  0
c8086EF: STA $002A
c8086F2: STA $1E82
c8086F5: LDA $0026,X       ; Layer priority data ; +1
c8086F9: STA $00DC
c8086FC: LDA $0028,X       ; Layer priority data ; +2
c808700: TAX
c808701: LDA #$0000
c808704: LDY #$2000
         PLB                                     ; +1
c808707: JSR $809F29       ; Load graphics data
         PHB
         JSR loadbank80                          ; -3
;80870B: LDX $10                                 ; -1
c80870D: LDA $002C,X       ; Player 1 Y pos      ;  0
c808711: CLC
c808712: ADC $0030,X       ; Player 2 Y pos      ; +1
c808716: LSR A
c808717: SEC
c808718: SBC #$0084
c80871B: TAY
c80871C: LDA $002A,X       ; Player 1 X pos      ; +2
c808720: CLC
c808721: ADC $002E,X       ; Player 2 X pos      ; +3
c808725: LSR A
c808726: SEC
c808727: SBC #$0080
c80872A: TAX
         PLB                                     ; +2
c80872B: JSR $80A9CC
c80872F: LDA #$7800
c808732: STA $1B7E
c808735: XBA
c808736: ORA #$0001
c808739: JSR $80A4D9
         PHB
         JSR loadbank80                          ; -2
;80873D: LDX $10                                 ;  0
c80873F: LDA $0010,X       ; Palette data        ; +1
c808743: TAY
c808744: LDA $0012,X       ; Palette data        ; +2
c808748: JSR $80A037       ; This function uses its own bank
c80874C: LDX $10                                 ; 
c80874E: LDA $0014,X       ; Sprite palette      ; +3
c808752: TAY
c808753: LDA $0016,X       ; Sprite palette      ; +4
         PLB                                     ; +3
c808757: JSR $80A05B
c80875B: JSR $80A079
         PHB
         JSR loadbank80                          ; -1
;80875F: LDX $10                                 ; +1
c808761: LDA $0018,X       ; Palette animation   ; +2
c808765: ORA $001A,X       ; Palette animation   ; +3
c808769: BEQ branch1
c80876B: LDA $001A,X       ; Palette animation   ; +4
c80876F: TAY
c808770: LDA $0018,X       ; Palette animation   ; +5
         PLB                                     ; +4
c808774: JSR $80825E
         PHB
         JSR loadbank80                          ;  0
;808778: LDX $10                                 ; +2
branch1:
c80877A: LDA $001C,X       ; Monster data        ; +3
c80877E: STA $00
;808780: LDA #$009F        ; 9F is the bank used, so I load the custom bank
         LDA $12                                 ; +4
c808783: STA $02
c808785: LDA #$80EC
c808788: LDY #$0081
         PLB                                     ; +3
c80878B: JSR $80825E
         PHB
         JSR loadbank80                          ; -1
;80878F: LDX $10                                 ; +1
c808791: LDA $001E,X       ; Victim data         ; +2
c808795: STA $00
;808797: LDA #$009F
         LDA $12                                 ; +3
c80879A: STA $02
c80879C: LDA #$81F6
c80879F: LDY #$0081
         PLB                                     ; +2
c8087A2: JSR $80825E
         PHB
         JSR loadbank80                          ; -2
;8087A6: LDX $10                                 ;  0
c8087A8: LDA $001E,X       ; Victim data         ; +1
c8087AC: TAX 
;8087AD: LDA #$009F
         LDA $12                                 ; +2
         PLB                                     ; +1
c8087B0: JSR $82DB46
         PHB
         JSR loadbank80                          ; -3
;8087B4: LDX $10                                 ; -1
c8087B6: LDA $0020,X       ; Item data           ;  0
c8087BA: STA $00
;8087BC: LDA #$009F
         LDA $12                                 ; +1
c8087BF: STA $02
c8087C1: LDA #$C8F6
c8087C4: LDY #$0080
         PLB                                     ;  0
c8087C7: JSR $80825E
c8087CB: CLC
c8087CC: LDA #$003C
c8087CF: ADC $10
c8087D1: PHA

loop:
c8087D2: WAI
c8087D3: WAI
c8087D4: WAI
c8087D5: PLA
c8087D6: TAX
c8087D7: CLC
c8087D8: ADC #$0008
c8087DB: PHA
         PHB
         JSR loadbank80_2 ;This version doesn't load X ; -4
c8087DC: LDA $0000,X                             ; -3
c8087E0: ORA $0002,X                             ; -2
c8087E4: BEQ return1
c8087E6: LDA $0004,X                             ; -1
c8087EA: STA $00
c8087EC: LDA $0006,X                             ;  0
c8087F0: STA $02
c8087F2: LDA $0002,X                             ; +1
c8087F6: TAY
c8087F7: LDA $0000,X                             ; +2
         PLB                                     ; +1
c8087FB: JSR $80825E
c8087FF: BRA loop
return1:
         PLB                                     ;  0 -- It fits exactly :'D
c808801: PLA
c808802: RTL

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; A second loading function starts here
; These are the loads for player start positions
org $808803
c808803: LDA $1E88
c808806: BEQ player2
;808808: LDX $10                                 ; +2
         PHB                                     ; +1
         JSR loadbank80_3                        ; -2
c80880A: LDA $002A,X ; Player 1 x position       ; -1
c80880E: STA $00
c808810: LDA $002C,X ; Player 1 y position       ;  0
c808814: STA $02
;808816: STZ $04     ; No longer needed because they are done in loadbank80_3
;808818: STZ $06                                 ; +4
c80881A: LDA #$CDF4
c80881D: LDY #$0080
         PLB                                     ; +3
c808820: JSR $808248

player2:
c808824: LDA $1E8A
c808827: BEQ return2
         PHB                                     ; +2
         JSR loadbank80                          ; -1
;808829: LDX $10                                 ; +1
c80882B: LDA $002E,X ; Player 2 x position       ; +2
c80882F: STA $00
c808831: LDA $0030,X ; Player 2 y position       ; +3
c808835: STA $02
c808837: LDA #$0001
c80883A: STA $04
c80883C: STA $06
c80883E: LDA #$CDF4
c808841: LDY #$0080
         PLB                                     ; +2
c808844: JSR $808248
return2:
c808848: RTL

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; This function is used to load data for the demos
org $809B14

;809B14: LDX $10                                 ; +2
         PHB
         JSR loadbank80                          ; -2
c809B16: LDA $0000,X                             ; -1
;809B1A: PHA                                     ;  0
         TAY                                     ; -1
c809B1B: LDA $0002,X                             ;  0
         PLB                                     ; -1
         PHY                                     ; -2
c809B1F: LDX #$007E
c809B22: LDY #$8000
c809B25: JSR $80CD20
c809B29: PLA
c809B2A: JSR $9C72
c809B2D: LDA #$0010
c809B30: JSR $808353
c809B34: LDA $6C
c809B36: JSR $8083D5
c809B3A: JSR $9C4A
c809B3D: JSR $808618
c809B41: JSR $8088A9
c809B45: JSR $808632
;809B49: JSR $8086A2    ; Moved to loadbank80_4  ; +2
;809B4D: LDX $10                                 ; +4
         PHB                                     ;
         JSR loadbank80_4                        ;  0
c809B4F: LDA $0032,X                             ; +1
c809B53: PHA
c809B54: LDA $0034,X                             ; +2
c809B58: TAX
c809B59: PLA
         PLB                                     ; +1
         NOP    ; Make up for the extra byte saved

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; A fourth loading function starts here.
; These are the loads for the level titles
org $808849
         PHB                                     ; -1
         JSR loadbank80                          ; -4
;808849: LDX $10                                 ; -2
c80884B: LDA $0036,X  ;Level title 1             ; -1
c80884F: TAY
c808850: LDA $0038,X  ;Level title 2             ;  0
c808854: TAX
c808855: LDA $10
;        PLB ; I don't load the bank, it will be loaded by the next function
c808857: JMP $82AC07

; A fifth function that is jumped to directly from the third
org $82AC09 ; This is right at the beginning
NOP         ; Removes a PHB, I already pushed the bank from the above function

org $82AC2B ; This is a spot in the middle of the original function
;82AC2B: LDA $136C
;82AC2E: AND #$007F ; Removed because the value is overwritten right away
;82AC31: STA $136C                               ; +9
c82AC34: LDA #$000F
c82AC37: STA $136C
c82AC3A: LDA #$0001
c82AC3D: STA $212C
;82AC40: LDA #$009F
         LDA $12                                 ; +10
c82AC43: PLX
c82AC44: JSR $82AD5A
c82AC48: JSR $AE6F
c82AC4B: JSR $82AD44
c82AC4F: STZ $0016
c82AC52: PLX
c82AC53: STX $0038
         PHB
         JSR loadbank82                          ; +6
c82AC56: LDA $0032,X ; Music                     ; +7
c82AC5A: PHA
c82AC5B: LDA $0034,X ; Unknown                   ; +8
c82AC5F: TAX
c82AC60: PLA
         PLB                                     ; +7
c82AC61: JSR $80CBD9
branch3:
c82AC65: LDA $0016
c82AC68: CMP #$0078
c82AC6B: BCC branch3
;82AC6D: LDA #$009F
         LDA $12                                 ; +8
c82AC70: PLX
c82AC71: JSR $82AD5A
c82AC75: JSR $AE6F
c82AC78: STZ $0016
         PHB
         JSR loadbank82                          ; +4
c82AC7B: LDX $0038
c82AC7E: LDA $0000,X ; Tileset                   ; +5
         TAY                                     ; +4
;82AC82: PHA                                     ; +5
c82AC83: LDA $0002,X ; Tileset                   ; +6
         PLB                                     ; +5
         PHY                                     ; +4
         NOP    ; Because I actually saved 4 bytes rewriting this thing
         NOP
         NOP
         NOP

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; Called at the end of the level to determine what bonuses should be given
org $82C259
c82C259: LDA $1E72
c82C25C: STA $005C
c82C25F: LDA $1E74
c82C262: STA $005E
c82C265: LDA $1E76
c82C268: STA $0060
;82C26B: LDA $1E78        ; Moved to loadbank82_2
;82C26E: STA $0062
;82C271: LDX $10
         PHB
         JSR loadbank82_2
c82C273: LDA $003A,X
         PLB
         CMP #$0000       ; Has to be inserted because PLB will override flags
c82C277: BNE branch4
c82C279: STZ $0044
c82C27C: STZ $0046
c82C27F: JSR $C56B
c82C282: BRA branch5

branch4:
c82C284: STA $0044
c82C287: CLC
c82C288: ADC #$0008
c82C28B: STA $0046
c82C28E: LDA #$C299
c82C291: STA $1EB2
c82C294: JSR $C56B
c82C297: BRA branch5

; For some reason there's another entry location in the middle here
org $82C299
;        PHB              ; All this stuff was moved to loadbank82_3
;        JSR loadbank82
;82C299: LDX $0044
;82C29C: LDA $0000,X
;        PLB
         JSR loadbank82_3
         CMP #$0000       ; Has to be inserted because PLB will override flags
c82C2A0: BEQ branch6
c82C2A2: INX
c82C2A3: INX
c82C2A4: STX $0044
c82C2A7: TAX
c82C2A8: JSR ($C346,X)

branch5:
c82C2AB: JSR $C2C6
c82C2AE: BCS branch6
c82C2B0: RTS

branch6:
c82C2B1: JSR $D44E
c82C2B4: JSR $C2C6
c82C2B7: LDY #$0000
c82C2BA: LDX #$C369
c82C2BD: LDA #$0082
c82C2C0: JSR $82B8FB
c82C2C4: SEC
c82C2C5: RTS