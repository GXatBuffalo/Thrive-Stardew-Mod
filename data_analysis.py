import json
import numpy as np
import matplotlib.pyplot as plt


cropsfile = "C:\GOG Games\Stardew Valley\Mods\Thrive\exported-data.json"
# load json
with open(cropsfile) as f:
    cropsdata = json.load(f)

requirements_list = []
depreciation_list = []
for c_id in cropsdata:
    c = cropsdata[c_id]
    requirements_list.append(c["Requirements"])
    depreciation_list.append(c["SoilDeprecation"])

print(requirements_list)
print(depreciation_list)
# suppose data["values"] = [numbers...]
requirements_list_np = np.array(requirements_list)

print(requirements_list_np)
values = ["nitro", "phos", "pH", "iridium", "mana"]

for x in range(0, 5):
    print("Analysis of: ", values[x])
    print("Mean:", np.mean(requirements_list_np[:, x]))
    print("Median:", np.median(requirements_list_np[:, x]))
    print("Max:", np.max(requirements_list_np[:, x]))
    print("Min:", np.min(requirements_list_np[:, x]))
    # plot
    plt.hist(requirements_list_np[:, x], bins=1)
    plt.show()