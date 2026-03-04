# Directional antenna deployment assistant (DADA)

### "A 45-year journey from a 10-year-old's dream to a global amateur radio tool."

DADA is an **open-source project** dedicated to helping ham radio operators calculate precise antenna bearings and distances. It has evolved from a 1980s dream into a comprehensive tool that bridges the gap between digital data and physical antenna control.

---

## Project versions

Currently, the DADA project is divided into two main branches:

### 1. Modern web version (v3.3)
A browser-based tool using HTML/JS and OpenStreetMap for global urban RF analysis.
* **Live Demo:** [http://dada1.decem.co.th/33.html](http://dada1.decem.co.th/33.html)

### 2. Legacy VB.NET version (v1.1 - v1.2)
The foundational Windows-based software that utilizes Thailand's Open Government Data.
* **Source Code:** Located in the `/v1.2_vb` directory.

---

## Getting started with DADA v1.1

To use the legacy Windows version, follow these steps:

1.  **Download data:** Get the official dataset from the [Government Data Portal](https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910).
2.  **Prepare file:** Save the Excel file as a **CSV** named `OpenGovernmentLatLongTambon.csv`.
3.  **Deploy:** Place the CSV in the same folder as the DADA executable.
4.  **Calculate:** * Enter your Home QTH (e.g., **"จตุจั"** for Chatuchak) and press **Enter**.
    * Press **F8** to set it as your home.
    * Search for any destination (e.g., **"นครป"** for Nakornpathom) to get the bearing and distance.

---

## History and motivation

The story of DADA began in **February 2009**, when I was first licensed as **E22JNE**. Faced with the challenge of effectively using a multi-beam directional antenna, I developed a matrix for every district in Thailand using Visual FoxPro.

The project took a significant leap in 2023 when I discovered the **Open Government Data**. By integrating this free dataset, I achieved a calculation accuracy with only **0.2% deviation** compared to my original 2009 matrices. This is well within the acceptable range for antennas like the 13-element Yagi-Uda, which has a **60° half-power beam-width**.

> **The spirit of ham radio:** This project is a tribute to the mentors (**HS1BFR** and **HS1KBG**) who passed on the spirit of radio to me on the 144.100 MHz band back in 2009.

---

## Project roadmap

We are currently moving towards **Directional antenna deployment automation**:

* **DADA2:** Hardware interfacing using Bluetooth and relays to automate rotator controls for KenPro and other brands.
* **DADA3:** Real-time Signal-to-Noise Ratio (SNR) and RSSI analysis via CAT/CIV protocols to find the peak signal position.
* **DADA31:** Implementing **Machine Learning** to interpret QRK levels (1-5) from QSO audio signals for automatic antenna adjustment.

---

## Contact and license

* **Author:** Pat "Jojo" Sadavongvivad (E25VBE)
* **Facebook:** [Pat Jojo Sadavongvivad](https://www.facebook.com/jojopat)
* **License:** **Public Domain** (The Unlicense). Feel free to develop, fork, and share for the advancement of the ham community.
