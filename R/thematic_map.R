# Voorbeelden gebruik van CBS Open Data v3 in R
# https://www.cbs.nl/nl-nl/onze-diensten/open-data
# Auteur: Jolien Oomens
# Centraal Bureau voor de Statistiek

# In dit voorbeeld worden gemeentegrenzen gekoppeld aan geboortecijfers om een 
# thematische kaart te maken.

library(cbsodataR)
library(tidyverse)
library(sf)

# Zoek op welke data beschikbaar is
metadata <- cbs_get_meta("83765NED")
print(metadata$DataProperties$Key)

# Download geboortecijfers en verwijder spaties uit regiocodes
data <- cbs_get_data("83765NED", 
                     select=c("WijkenEnBuurten","GeboorteRelatief_25")) %>%
  mutate( WijkenEnBuurten = str_trim(WijkenEnBuurten),
          geboorte = GeboorteRelatief_25)

# Haal de kaart met gemeentegrenzen op van PDOK
gemeentegrenzen <- st_read("https://geodata.nationaalgeoregister.nl/cbsgebiedsindelingen/wfs?request=GetFeature&service=WFS&version=2.0.0&typeName=cbs_gemeente_2017_gegeneraliseerd&outputFormat=json")

# Koppel CBS-data aan geodata met regiocodes
data <- 
  gemeentegrenzen %>%
  left_join(data, by=c(statcode="WijkenEnBuurten"))

# Maak een thematische kaart
data %>%
  ggplot() +
  geom_sf(aes(fill = geboorte)) +
  scale_fill_viridis_c() +
  labs(title = "Levend geborenen per 1000 inwoners, 2017", fill = "") +
  theme_void()
