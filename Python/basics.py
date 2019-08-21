"""
Voorbeelden gebruik van CBS Open Data v3 in Python
https://www.cbs.nl/nl-nl/onze-diensten/open-data
Auteur: Jolien Oomens
Centraal Bureau voor de Statistiek

Minimale voorbeelden van het ophalen van een tabel, het koppelen van metadata
en het filteren van data voor het downloaden.
"""

import pandas as pd
import cbsodata

# Downloaden van tabeloverzicht
toc = pd.DataFrame(cbsodata.get_table_list())

# Downloaden van gehele tabel (kan een halve minuut duren)
data = pd.DataFrame(cbsodata.get_data('83765NED'))
print(data.head())

# Downloaden van metadata
metadata = pd.DataFrame(cbsodata.get_meta('83765NED', 'DataProperties'))
print(metadata[['Key','Title']])

# Downloaden van selectie van data
data = pd.DataFrame(
        cbsodata.get_data('83765NED', 
                          filters="WijkenEnBuurten eq 'GM0363    '",
                          select=['WijkenEnBuurten','AantalInwoners_5']))
print(data.head())
