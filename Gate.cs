using UnityEngine;
using System.Collections;

public class BukaGerbang : MonoBehaviour
{
    public float jarakNaik = 5f;
    public float kecepatanGerak = 2f;

    private Vector3 posisiAwal;
    private Vector3 posisiTujuan;
    private bool sedangBergerak = false;
    private bool apakahGerbangTerbuka = false;

    void Start()
    {
        posisiAwal = transform.position;
        posisiTujuan = posisiAwal + new Vector3(0, jarakNaik, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !sedangBergerak)
        {
            if (!apakahGerbangTerbuka)
            {
                StartCoroutine(NaikkanGerbang());
            }
            else
            {
                StartCoroutine(TurunkanGerbang());
            }
        }
    }

    private IEnumerator NaikkanGerbang()
    {
        sedangBergerak = true;

        while (transform.position != posisiTujuan)
        {
            transform.position = Vector3.MoveTowards(transform.position, posisiTujuan, kecepatanGerak * Time.deltaTime);
            yield return null;
        }

        apakahGerbangTerbuka = true;
        sedangBergerak = false;
    }

    private IEnumerator TurunkanGerbang()
    {
        sedangBergerak = true;

        while (transform.position != posisiAwal)
        {
            transform.position = Vector3.MoveTowards(transform.position, posisiAwal, kecepatanGerak * Time.deltaTime);
            yield return null;
        }

        apakahGerbangTerbuka = false;
        sedangBergerak = false;
    }
}