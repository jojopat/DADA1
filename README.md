# DADA1
Directional Antenna Deployment Assitant 1 - calculate angle and distant between two ham station

BEFORE USE : download open Government data from this link : 

https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910

- Open the Excel file and save it as Comma Separated Value (.CSV) file under the name OpenGovernmentLatLongTambon.csv
- Place this CSV in the same folder as DADA exe
- Run DADA exe, the app will read CSV into memory and ready to use
- Type in your home QTH into the textbox "จตุจั" and press enter. Chatuchak coordinate will appear
- Press F8 to set Chatuchak as your home QTH
- Type in any tambon or ampur in part or whole eg. "นครป" then Enter button to get Nakornpathom with angle and air displacement between Chatuchak and Nakornpathom.

Contact info :  Thai ham : E25VBE 
                facebook : Pat Jojo Sadavongvivad


Development History of DADA - Directional Antenna Deployment Assistant

First idea : Feb 2009, when I first acquire beginner ham operator license and E22JNE callsign.  Right after 
decided to install directional antenna multi-beam 2x2 192E from ClubStationThai antenna maker. While waiting 
for tower and antenna installation, I started to think further of how to effectively use it. Then wrote Visual Foxpro
code to create maxtrix table of every district in Thailand by taking the coordinate of every district office to to 
create :

    1. Distance matrix
    2. Angle matrix

Then distribute the angle matrix in JPG for A4 and A0 paper format through ClubStationThai antenna manufacturer by 
HS1BFR Prasit Sottipattanapong.

Later in 2019, while reading this news of how heavily calculation ham operator has to perform to calculate 
the angle to rotate the antenna in order to communicate with aboard HTMS Chakrinarubet while operating HS10KING/MM
that I decide to write this application but cannot distribute due to the coordinate data is a copyrighted dataset of 
commercial organization.  Just early July 2023 that E20FWF warned me about expired license will lead to big 
fine after all ham gears turned illegal.  So I rush to renew my license to receive the new callsign E25VBE.  Then
restart ham activity and reconnect all ham gears. Then start to think seriously of how to distribute this piece of 
code for the use of others.  And since I am RAST lifetime member, I brought this idea to discuss with E20EHQ at RAST 
and he's very much happy about the idea.  After that, while googling around, I found the Open Government Data from 
this website :

https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910

It's Lat/Long data of every tambon and ampur in Thailand as Open Government Data where everyone can download and 
free to use.  So I begin to fix the code to utilize this open Government dataset. Then test it against the angle
and distant matrix created back in 2009.  Testing the result of new dataset against 2009 matrix has an average of
only 0.2% deviation from the previous data so it is very acceptable.  Sharpest directional antenna I know is 
multi-beam 2x2 192E only give 30 degrees half-power beam-width. And 13 elements yagi-uda antenna has 60 degrees 
half-power beam-width so 0.2% deviation in the angle is very much acceptable with this dataset.

The main part of the app to calculate distance and angle between two stations has been done.  Next step of the 
development is to study to prepare some hardware to interface to antenna rotator like KenPro or other brands to 
integrate DADA with CAT-protocol supported tranceivers in order to do final adjustment of the direction to find 
peak signal strength.  Or to add some other features in the future.

Feel free to further develop this code for the advancement of the hams.  Kindly keep me inform so to spread 
such happiness around ^_^

E25VBE or Pat Jojo Sadavongvivad on facebook.com

Without kind words from HS1BFR and HS1KBG on 144.100 MHz ham band back in 2009 I would not have started all this
activity.  They show and pass to me the spirits of the hams and also continuously empower me to continue to develop 
over the years.  All thanks as a result of the usefulness of DADA is what I learn from both of them.
