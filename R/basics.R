# Voorbeelden gebruik van CBS Open Data v3 in R
# https://www.cbs.nl/nl-nl/onze-diensten/open-data
# Auteur: Jolien Oomens
# Centraal Bureau voor de Statistiek

# Minimale voorbeelden van het ophalen van een tabel en metadata
# met behulp van het package cbsodataR.

# Eenmalig uitvoeren:
# install.packages("cbsodataR")

library(cbsodataR)

# Downloaden van tabeloverzicht
toc <- cbs_get_toc()
head(toc)

# Downloaden van gehele tabel (kan een halve minuut duren)
data <- cbs_get_data("83765NED")
head(data)

# Downloaden van metadata
metadata <- cbs_get_meta("83765NED")
head(metadata)

# Downloaden van selectie van data
data <- cbs_get_data("83765NED", 
                     WijkenEnBuurten = "GM0363    ",
                     select = c("WijkenEnBuurten", "AantalInwoners_5"))
head(data)