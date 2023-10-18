# DADA1
Directional Antenna Deployment Assitant 1 - calculate angle and distant between two ham station

BEFORE USE : download open Government data from this link : 

https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910

- Open the Excel file and save it as Comma Separated Value (.CSV) file under the name OpenGovernmentLatLongTambon.csv
- Place this CSV in the same folder as DADA exe
- Run DADA exe, the app will read CSV into memory and ready to use

Contact info :  Thai ham : E25VBE 
                facebook : Pat Jojo Sadavongvivad


Development History of DADA - Directional Antenna Deployment Assistant

First idea : Feb 2009, when I first acquire beginner ham operator license and E22JNE callsign.  Right after 
decided to install directional antenna multi-beam 2x2 192E from ClubStationThai antenna maker. While waiing 
for the installation, I think further of how to effectively use it.  Then create maxtrix table of every district 
in Thailand by taking the coordinate of every district office to to create :

    1. Distance matrix
    2. Angle matrix

Then distribute the angle matrix in JPG/PDF through ClubStationThai antenna by HS1BFR Prasit Sottipattanapong

Later in 2019, right after reading this news of how heavily calculation ham operator has to be done to calculate 
the angle to rotate the antenna in order to communicate with aboard HTMS Chakrinarubet that I decide to write 
this application but cannot distribute due to the coordinate data is a copyrighted dataset of commercial 
organization.  Just early October 2023 that E20FWF rang me to warn about expired license will lead to big fine 
from all ham gears turned illegal.  So I rush to renew my license to receive the new callsign E25VBE.  Then
restart ham activity and reconnect all ham gears. Then start to think seriously of how to distribute this 
piece of code for the use of others.  Bring this idea to discuss with E20EHQ at RAST about this.  Then while 
googling around, I found the Open Government Data from this website :

https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910

Which contain Lat/Long data of every Tambon in Thailand as Open Government Data where everyone can download and 
free to use.  So I begin to fix the code to utilize this open Government dataset.

The main part of the app to calculate distance and angle between two stations has been done.  Next step of the 
development is to study to prepare some hardware to interface to antenna rotator like KenPro or other brands to 
integrate DADA with CAT-protocol supported tranceivers in order to do final adjustment of the direction to find 
peak signal strength.  Or to add some other features in the future.

Feel free to further develop this code for the advancement of the hams.  Kindly keep me inform so to spread 
such happiness around ^_^

E25VBE or Pat Jojo Sadavongvivad on facebook.com

Without kind words from HS1BFR and HS1KBG on 144.100 MHz ham band back in 2009 I would have start all this.
They show and pass to me the spirits of the ham and also continuously empower me to continue to develop over the 
years.  All thanks as a result of the usefulness of DADA also a what I learn from both of them.
