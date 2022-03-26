using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Data;

using System.Linq;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string fishName = "½¬¸®";
        Debug.Log(Random.Range((from row in FishingData.FishDataTable.AsEnumerable()
                                where (string)row["Name"] == fishName
                                select row.Field<float>("MinSize")).ElementAt<float>(0),    
                             (from row in FishingData.FishDataTable.AsEnumerable()
                              where (string)row["Name"] == fishName
                              select row.Field<float>("MaxSize")).ElementAt<float>(0)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
