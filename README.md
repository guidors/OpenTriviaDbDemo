# OpenTriviaDbWebService

Voorbeeld Vue Client met een caching webservice voor opentdb.com

## project
Het project bestaat uit 3 onderdelen:
- OpenTriviaDbWebService
<br>De webservice die voorziet in communicatie met de opentdb.com backend via de OpenTriviaDbConnector.
<br>De OpenTriviaDbWebServiceController biedt 3 service: get_quiz, check_answers en get_categories
- OpenTriviaDbWebService.Tests
<br>Unit tests voor de OpenTriviaDbConnector
- OpenTriviaDbFrontEnd
<br>Een vue client die de webservice aanroept.
<br>Grotendeels gegenereerd met AI. Het framework is wel een waar ik ervaring mee heb en makkelijk kan aanpassen.

## draaien en deployment
Het Visual Studioproject zou met het profiel OpenTriviaDb meteen moeten opstarten.
<br>De service wordt dan met Swaggerpagina gestart en een browser met de Vue frontend.

## release maken
In project OpenTriviaDbWebService voer uit: `dotnet publish -c Release`
<br>Release kan vervolgens worden gevonden in de folder bin\Release\net9.0\publish
<br>
<br>Voor release van frontend voer `npm run build`  uit in de opentriviadbfrontend folder.
<br>resultaat is te vinden in de dist folder. 
<br>In het bestand src/services/api.ts kan van tevoren de Base URL voor de backend worden geconfigureerd.


Draaiende demo: https://www.xilongo.eu/quiz/

