"""
Voorbeelden gebruik van CBS Open Data v3 in Python
https://www.cbs.nl/nl-nl/onze-diensten/open-data
Auteur: Jolien Oomens
Centraal Bureau voor de Statistiek

In dit voorbeeld worden gemeentegrenzen gekoppeld aan geboortecijfers om een 
thematische kaart te maken.
"""

import pandas as pd
import geopandas as gpd
import cbsodata

# Zoek op welke data beschikbaar is
metadata = pd.DataFrame(cbsodata.get_meta('83765NED', 'DataProperties'))


# Download geboortecijfers en verwijder spaties uit regiocodes
data = pd.DataFrame(cbsodata.get_data('83765NED', select = ['WijkenEnBuurten', 'Codering_3', 'GeboorteRelatief_25']))
data['Codering_3'] = data['Codering_3'].str.strip()

# Download geboortecijfers en verwijder spaties uit regiocodes
data = pd.DataFrame(cbsodata.get_data('83765NED', select = ['WijkenEnBuurten', 'Codering_3', 'GeboorteRelatief_25']))
data['Codering_3'] = data['Codering_3'].str.strip()

# Haal de kaart met gemeentegrenzen op van PDOK
geodata_url = 'https://geodata.nationaalgeoregister.nl/cbsgebiedsindelingen/wfs?request=GetFeature&service=WFS&version=2.0.0&typeName=cbs_gemeente_2017_gegeneraliseerd&outputFormat=json'
gemeentegrenzen = gpd.read_file(geodata_url)

# Koppel CBS-data aan geodata met regiocodes
gemeentegrenzen = pd.merge(gemeentegrenzen, data,
                           left_on = "statcode", right_on = "Codering_3")

# Maak een thematische kaart
p = gemeentegrenzen.plot(column='GeboorteRelatief_25', figsize = (10,8))
p.axis('off')
p.set_title('Levend geborenen per 1000 inwoners, 2017')
