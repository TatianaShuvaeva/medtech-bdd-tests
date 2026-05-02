Feature: Patientenakte
	Als Arzt
	Möchte ich Blutdruckverläufe einsehen
	Damit ich Hypertonie-Risiken frühzeitig erkennen kann

	@smoke @patient-chart
	Scenario: Arzt sieht Blutdruckverlauf des Patienten
		Given Patient "Klaus Bauer" hat folgende Blutdruckmessungen:
			| Datum      | Systolisch | Diastolisch |
			| 2024-01-10 | 145        | 92          |
			| 2024-01-17 | 138        | 88          |
			| 2024-01-24 | 150        | 95          |
		When der Arzt das Blutdruckdiagramm öffnet
		Then sollte der durchschnittliche Systolwert 144 betragen
		And der Trend sollte als "STEIGEND" markiert sein
		And ein klinischer Hinweis sollte angezeigt werden: "Hypertonie Stufe 1"
