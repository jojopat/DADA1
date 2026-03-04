* =============================================================================================
* Project: Directional Antenna Deployment Assistant (DADA) v1.0
* Original Author: Pat "Jojo" Sadavongvivad (E25VBE previously E22JNE)
* Development Era: Circa 2009 (Visual FoxPro / Foxbase)
* Purpose: To generate a Distance & Angle Matrix for Thai provinces using coordinate data.
* =============================================================================================

clos all
sele 1
use ampur
sele 2
use ampur2

* --- Initialize Output File ---
set print to
erase angleprov.txt
set print to angleprov.txt
set prin on

* --- Phase 1: Generate Column Headers ---
sele 1
go top
?"Prov,"
do while !eof()
	prov=trim(subs(left(name,at(",",name)-1),8))
	ampur=trim(subs(subs(name,at(",",name)),8))
	if prov#ampur.and.!INCLUDE
		skip
		loop
	endif

	??trim(subs(name,7+at(",",name)))
	skip
	??iif(!eof(),",","")
enddo
go top

* --- Phase 2: Generate Matrix Data (Nested Loop) ---
do while !eof()
	prov=trim(subs(left(name,at(",",name)-1),8))
	ampur=trim(subs(subs(name,at(",",name)),8))
	if prov#ampur.and.!INCLUDE
		skip
		loop
	endif
	r1=recno()
	?prov,","

	sele 2
	go top
	do while !eof()
		prov=trim(subs(left(name,at(",",name)-1),8))
		ampur=trim(subs(subs(name,at(",",name)),8))
		if prov#ampur.and.!INCLUDE
			skip
			loop
		endif

		if recno()#r1
			* Fetch coordinates from Database A and Database B
			x1=a->n2
			x2=b->n2
			y1=a->n3
			y2=b->n3
			
			* Calculate Delta X and Delta Y
			xd=x2-x1
			yd=y2-y1
			
			* Calculate Angle (Bearing) using Atan2
			* mdeg = ArcTangent(dy, dx) converted to degrees
			mdeg=rtod(atn2( yd, xd ))
			
			* --- Compass Bearing Normalization ---
			* Converts standard Cartesian angle to Compass Heading (North = 0)
			do case
			case mdeg>=0 .and. mdeg<=90
				hdeg=90-mdeg
			case mdeg<0 .and. mdeg>-180
				hdeg=-(mdeg-90)
			case mdeg>90 .and. mdeg<=180
				hdeg=450-mdeg
			endcase
			??str(hdeg,4,0)+","
		else
			* Self-referencing cell
			??"     ,"
		endif
		skip
	enddo
	sele 1
	skip
enddo

set prin off
set prin to
