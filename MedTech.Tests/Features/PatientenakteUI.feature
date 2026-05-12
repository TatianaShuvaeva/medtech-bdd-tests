# language: de
Funktionalität: Blazor UI-Tests (Playwright)
	Als Arzt
	Möchte ich die Benutzeroberfläche der MedTech-Anwendung nutzen
	Damit ich Patienten suchen, Akten einsehen und Medikamente sicher verschreiben kann

	@browser @smoke
	Szenario: Arzt sucht Patient in der Patientenliste
		Angenommen Dr. Müller ist in der Blazor-Anwendung eingeloggt
		Wenn sie nach Patient "Schmidt" sucht
		Dann sollte mindestens ein Suchergebnis angezeigt werden
		Und der erste Treffer sollte "Schmidt" enthalten

	@browser @smoke
	Szenario: Arzt öffnet Patientenakte
		Angenommen Dr. Müller ist in der Blazor-Anwendung eingeloggt
		Wenn sie nach Patient "Hans Schmidt" sucht
		Dann sollte die Patientenakte angezeigt werden

	@browser @sicherheit
	Szenario: Allergie-Warnung erscheint bei bekanntem Allergen in der UI
		Angenommen Dr. Müller ist in der Blazor-Anwendung eingeloggt
		Wenn sie nach Patient "Anna Klein" sucht
		Und die Patientenakte angezeigt werden
		Und der Arzt "Amoxicillin" in der UI verschreibt
		Dann sollte eine rote Allergie-Warnung in der UI erscheinen
		Und der Schweregrad der Warnung sollte "HOCH" sein

	@browser
	Szenario: Erfolgreiches Verschreiben ohne Allergie in der UI
		Angenommen Dr. Müller ist in der Blazor-Anwendung eingeloggt
		Wenn sie nach Patient "Max Müller" sucht
		Und die Patientenakte angezeigt werden
		Und der Arzt "Ibuprofen" in der UI verschreibt
		Dann keine Allergie-Warnung sollte erscheinen
		Und eine Erfolgsmeldung sollte in der UI erscheinen

	
	@browser @regulatorisch
	Szenario: Erfolgreiche Verschreibung wird in der DB persistiert
		Angenommen Patient "Max Müller" existiert in der Datenbank
		Und der Patient hat bisher 0 Rezept(e)
		Und Dr. Müller ist in der Blazor-Anwendung eingeloggt
		Wenn sie nach Patient "Max Müller" sucht
		Und die Patientenakte angezeigt werden
		Und der Arzt "Ibuprofen" in der UI verschreibt
		Dann eine Erfolgsmeldung sollte in der UI erscheinen
		Und in der Datenbank korrekt gespeichert ist
		Und sollte die Rezeptanzahl des Patienten um 1 gestiegen sein
