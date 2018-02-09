/*
    VisualGravityWells
    Visualisation of Gravity Field Wells of the Sun, Earth and Moon. 
    Copyright (C) 2018  Barnabás Nagy - otapiGems.net - otapiGems@protonmail.ch

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldEngine : MonoBehaviour
{
    Terrain terr; // terrain to modify 
    float maxmass; // maximum mass
    //real s = 1.72e17;
    int fieldSize = 1000; // [unit] Diamater size of the World
    int gravityFieldCount = 10; // [count] Count of nodes of gravity field

    int groundlevel = 1000; // this is the ground level
    float G = 6.67f;
    // Use this for initialization
    void Start()
    {
        terr = Terrain.activeTerrain;
        gravityFieldCount = terr.terrainData.heightmapWidth;
        fieldSize = Mathf.RoundToInt(terr.terrainData.size.x);
        // find max mass
        maxmass = 0;
        foreach (Transform obj in transform)
        {
            if (maxmass < obj.GetComponent<Rigidbody>().mass)
            {
                maxmass = obj.GetComponent<Rigidbody>().mass;
            }
        }

        
    }
    // Update is called once per frame
    void Update()
    {
        updateGravityField();
    }

    void updateGravityField()
    {
        // reset heights to groundlevel
        float[,] allheights = terr.terrainData.GetHeights(0, 0, gravityFieldCount, gravityFieldCount);
        for (int i = 0; i < gravityFieldCount; i++)
        {
            for (int j = 0; j < gravityFieldCount; j++)
            {
                allheights[i, j] = 1;
            }
        }

        foreach (Transform obj in transform)
        {
            // get the normalized position of this game object relative to the terrain
            Vector3 tempCoord = (obj.position - terr.gameObject.transform.position);
            Vector3 coord;
            coord.x = tempCoord.x / terr.terrainData.size.x;
            coord.y = tempCoord.y / terr.terrainData.size.y;
            coord.z = tempCoord.z / terr.terrainData.size.z;

            // get the position of the terrain heightmap where this game object is
            int posXInTerrain = (int)(coord.x * gravityFieldCount);
            int posZInTerrain = (int)(coord.z * gravityFieldCount);

            Vector2 posCenter = new Vector2(posZInTerrain, posXInTerrain);
            float mass = obj.GetComponent<Rigidbody>().mass;
            float radius = (obj.transform.localScale.x / 2);
            for (int x = 0; x < gravityFieldCount; x++)
            {
                for (int z = 0; z < gravityFieldCount; z++)
                {
                    Vector2 point = new Vector2(x, z);
                    float gravity = 0;
                    float distance = Vector2.Distance(posCenter, point);


                    if (distance <= radius)
                    {
                        distance = radius;
                    }

                    gravity = mass / (distance * distance / 5000);
                    allheights[x, z] = allheights[x, z] - (gravity / maxmass);
                }
            }
        }
        terr.terrainData.SetHeights(0, 0, allheights);
    }


}
