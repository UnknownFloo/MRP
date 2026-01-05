# MRP - Media Rating Platform

## Übersicht
MRP (Media Rating Platform) ist eine moderne Web-API-Anwendung, die es Benutzern ermöglicht, Medien zu bewerten, zu kommentieren und Empfehlungen zu erhalten. Die Anwendung verwendet eine Model-basierte Architektur mit PostgreSQL als Datenbank.

## Features
- **🔐 Benutzerauthentifizierung**: Registrierung und Anmeldung mit Token-basierter Authentifizierung
- **📱 Media Management**: Vollständiges CRUD für Medien (Filme, Serien, etc.)
- **⭐ Bewertungssystem**: Sterne-Bewertungen (1-5) und Kommentare mit Bestätigungssystem
- **❤️ Favoriten**: Medien als Favoriten markieren und verwalten
- **🏆 Leaderboard**: Ranglisten der aktivsten Bewerter
- **🎯 Empfehlungen**: Intelligente Empfehlungen basierend auf Genre und Medientyp
- **👤 Benutzerprofile**: Umfassende Profilverwaltung mit Bewertungshistorie
- **👍 Like-System**: Kommentare liken und unliken

## Technologie Stack
- **.NET 9.0**: Backend-Framework
- **C#**: Programmiersprache
- **PostgreSQL**: Datenbank mit JSONB-Unterstützung für Genres
- **Npgsql**: PostgreSQL-Connector für .NET
- **Docker**: Containerisierung und Entwicklungsumgebung

## Projektstruktur
```
MRP-API/                     # Haupt-API-Projekt
├── Endpoints/               # API-Endpunkte (organisiert nach Features)
│   ├── Auth/               # Authentifizierung (Login, Register)
│   ├── Media/              # Media-Management (CRUD, Search)
│   ├── Rating/             # Bewertungssystem (Rate, Update, Like, Confirm)
│   ├── Favorite/           # Favoriten (Add/Remove)
│   ├── Users/              # Benutzer (Profile, Favorites, History)
│   ├── Leaderboard/        # Ranglisten
│   └── Recommendations/    # Empfehlungen (by Genre/Content)
├── Models/                 # Datenmodelle
│   ├── Media.cs           # Media-Entität
│   ├── Rating.cs          # Rating-Entität  
│   └── User.cs            # User-Entität
├── Utils/                  # Hilfsfunktionen
│   ├── TokenValidation.cs # JWT-Token Validierung
│   └── GetUsername.cs     # Username aus Token extrahieren
├── Codes/                  # HTTP-Response-Handler
│   ├── 2xx.cs             # Success-Responses
│   ├── 4xx.cs             # Client-Error-Responses
│   └── 5xx.cs             # Server-Error-Responses
└── GlobalUsings.cs        # Globale Namespaces

MRP-API.Tests/              # Unit-Tests (Mirror der Hauptstruktur)
init-scripts/               # PostgreSQL-Initialisierung
├── 01-init.sql           # Datenbankschema und Testdaten
docker-compose.yml          # PostgreSQL-Container Setup
```

## Installation und Setup

### Voraussetzungen
- .NET 9.0 SDK
- Docker (für PostgreSQL)
- Git

### Lokale Entwicklung

1. **Repository klonen**
   ```bash
   git clone <repository-url>
   cd "MRP"
   ```

2. **PostgreSQL mit Docker starten**
   ```bash
   docker-compose up -d
   ```
   
3. **Abhängigkeiten installieren**
   ```bash
   dotnet restore
   ```

4. **Anwendung starten**
   ```bash
   dotnet run --project MRP-API
   ```

Die API läuft standardmäßig auf `http://localhost:8000`

### Datenbank-Details
- **Host**: localhost:5432
- **Database**: mrp_database  
- **Username**: mrp
- **Password**: mrp1234

Die Datenbank wird automatisch mit Testdaten initialisiert (siehe `init-scripts/01-init.sql`).

## API-Endpunkte

### 🔐 Authentifizierung
- `POST /auth/register` - Neuen Benutzer registrieren
- `POST /auth/login` - Benutzer anmelden (Bearer Token erhalten)

### 📱 Media Management  
- `POST /media/create` - Neues Medium erstellen
- `GET /media/view?mediaId=<id>` - Medium anzeigen
- `PUT /media/update?mediaId=<id>` - Medium aktualisieren
- `DELETE /media/delete?mediaId=<id>` - Medium löschen
- `GET /media/search?title=<title>&genre=<genre>...` - Medien suchen

### ⭐ Bewertungen
- `POST /rating/rate?mediaId=<id>` - Medium bewerten
- `PUT /rating/update?ratingId=<id>` - Bewertung aktualisieren  
- `POST /rating/like?ratingId=<id>` - Kommentar liken/unliken
- `POST /rating/confirm?ratingId=<id>` - Kommentar bestätigen (nur Media-Ersteller)

### ❤️ Favoriten
- `POST /favorite/add?mediaId=<id>` - Zu Favoriten hinzufügen
- `DELETE /favorite/remove?mediaId=<id>` - Aus Favoriten entfernen

### 👤 Benutzer
- `GET /users/profile?username=<name>` - Benutzerprofil anzeigen
- `PUT /users/update` - Eigenes Profil aktualisieren
- `GET /users/favorites` - Eigene Favoriten abrufen
- `GET /users/ratings` - Eigene Bewertungshistorie abrufen

### 🏆 & 🎯 Sonstiges
- `GET /leaderboard` - Benutzer-Rangliste nach Anzahl Bewertungen
- `GET /recommendations/content?mediaType=<type>` - Empfehlungen nach Medientyp
- `GET /recommendations/genre?genre=<genre>` - Empfehlungen nach Genre

### Authentifizierung
Alle Endpunkte (außer Login/Register) erfordern einen `Authorization` Header:
```
Authorization: <username>-mrpToken
```

## Tests ausführen
```bash
# Alle Tests
dotnet test
# Bedenke das einige Test möglicherweise Fehlschlagen da sie versuchen Daten zu löschen welche nicht existieren
```

## Entwicklung

### Code-Stil
- **Models**: Lowercase property names (entspricht DB-Schema)
- **Endpoints**: Eine Klasse pro Endpoint mit statischen Handler-Methoden
- **Error Handling**: Strukturierte HTTP-Response-Handler in `Codes/`
- **Database**: PostgreSQL mit Npgsql, parametrisierte Queries

### Neue Endpoints hinzufügen
1. Endpoint-Klasse in entsprechendem `Endpoints/` Ordner erstellen
2. Statische Handler-Methode mit `HttpListenerRequest/Response` implementieren
3. Token-Validierung über `TokenValidation.IsTokenValid()` 
4. Models für typisierte Datenverarbeitung verwenden
5. Tests in `MRP-API.Tests/` hinzufügen

## Deployment
```bash
# Docker Image erstellen
docker-compose up -d

# API Builden
dotnet build --project MRP-API

```
