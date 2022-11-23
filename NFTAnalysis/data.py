from moralis import evm_api
import numpy as np
import pandas as pd
from pandas import json_normalize
import matplotlib.pyplot as plt
import datetime
import time


api_key = "xxx"

cursor = ''
df = pd.DataFrame()

for x in range(2):

    result = evm_api.nft.get_nft_contract_transfers(
        api_key=api_key,
        params={
            "address": "0xb47e3cd837dDF8e4c57F05d70Ab865de6e193BBB", 
            "chain": "eth",
            "cursor": cursor
        }
    )

    cursor = result["cursor"]
    df2 = json_normalize(result['result'])

    if df.empty:
      df = df2
    else:
      df = pd.concat([df, df2])
    
    time.sleep(1.1)

print(df)
df = df[df['value'] != '0']

df['Date'] = df.apply(lambda row: datetime.datetime.strptime(row.block_timestamp[0:10],'%Y-%m-%d').strftime('%b %d'), axis=1)

dates = df.Date.unique()[0:7]

dates = dates[::-1]

volumes = []
avgs = []

for date in dates:
    tempDf = df[df.Date == date]
    values = [int(num)/1000000000000000000 for num in tempDf['value']]
    volumes.append(np.sum(values))
    avgs.append(np.mean(values))

plt.style.use('dark_background')

fig,ax = plt.subplots()
fig.set_facecolor('#202225')
ax.set_facecolor("#202225")
ax.grid(b=True, axis="y", zorder=0, color="#36383F")
ax.bar(x=dates, height=volumes, width=0.4, zorder=3, color="#E6E8EB")
ax.yaxis.set_major_locator(plt.MaxNLocator(3))
ax.set_ylabel("Volume (ETH)", fontweight='heavy', labelpad=20)
ax.spines['top'].set_visible(False)
ax.spines['right'].set_visible(False)
ax.spines['bottom'].set_visible(False)
ax.spines['left'].set_visible(False)

ax2 = ax.twinx()
ax2.plot(dates, avgs, color='#5B60E0')
ax2.yaxis.set_major_locator(plt.MaxNLocator(3))
ax2.set_ylabel("Average Price (ETH)", fontweight='heavy', rotation=270, labelpad=20)
ax2.spines['top'].set_visible(False)
ax2.spines['right'].set_visible(False)
ax2.spines['bottom'].set_visible(False)
ax2.spines['left'].set_visible(False)

plt.title('Volume and price (CryptoPunks)', loc='left', fontsize=16, fontweight="heavy", verticalalignment="top")
plt.show() 
