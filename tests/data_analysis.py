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
depreciation_list_np = np.array(depreciation_list)

values = ["nitro", "phos", "pH", "iridium", "mana"]

for x in range(0, 5):
    print("Analysis of: ", values[x])
    print("Mean:", np.mean(requirements_list_np[:, x*2]))
    print("Median:", np.median(requirements_list_np[:, x*2]))
    print("Max:", np.max(requirements_list_np[:, x*2]))
    print("Min:", np.min(requirements_list_np[:, x*2]))
    # plot
    plt.title("Requirements: " + values[x])
    plt.ylabel("Y-axis: Range")
    plt.xlabel("X-axis: Count")
    plt.hist(requirements_list_np[:, x*2], bins=200)
    plt.show()

for x in range(0, 5):
    print("Analysis of: ", values[x])
    print("Mean:", np.mean(depreciation_list_np[:, x]))
    print("Median:", np.median(depreciation_list_np[:, x]))
    print("Max:", np.max(depreciation_list_np[:, x]))
    print("Min:", np.min(depreciation_list_np[:, x]))

    plt.title("Soil Depreciation: " + values[x])
    plt.ylabel("Y-axis: Range")
    plt.xlabel("X-axis: Count")
    plt.hist(depreciation_list_np[:, x], bins=200)
    plt.show()