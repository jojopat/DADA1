# Legacy source code: DADA v1.0 (Foxbase)

### "The digital foundation created in 2009."

This directory contains the original **Foxbase/Visual FoxPro** source code that started the DADA journey in 2009. At the time, the project was designed to generate high-accuracy Distance and Angle Matrices for amateur radio operators across Thailand.

---

## Historical context
The code was developed to calculate the bearing needed to rotate a multi-beam directional antenna. The resulting matrices were distributed to the Thai ham community through **ClubStationThai** to help operators find the correct heading for various provinces.

---

## Technical legacy
The angle calculation logic found in `angleprov.prg` is the direct ancestor of the current **DADA v3.3** (Web Version). Despite the shift from local databases to modern APIs, the core mathematical normalization of Cartesian angles to compass bearings remains consistent, proving the robustness of the original design.

* **Files included:**
    * `angleprov.prg`: The main calculation script.
    * `Prov.dbf`: The original coordinate database format (dBase).

## Sample output

The file `angleprov.txt` is the direct result of running `angleprov.prg` back in 2009. It contains:
* **The Angle Matrix:** A comma-separated table showing the calculated bearing between Thai provinces.
* **Historical Accuracy:** This specific output serves as the reference point for the modern DADA v3.3, which maintains a high level of consistency with only **0.2% deviation** from these original calculations.
---

## Author
* **Pat "Jojo" Sadavongvivad (E25VBE)**
* **License:** Public Domain (The Unlicense)
