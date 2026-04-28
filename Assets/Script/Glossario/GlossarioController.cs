using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlossarioController : MonoBehaviour
{
    public TextMeshProUGUI TitoloImp;
    public TextMeshProUGUI colore;
    public TextMeshProUGUI profumo;
    public TextMeshProUGUI periodo;
    public TextMeshProUGUI corolla;
    public TextMeshProUGUI ricompensa;

    private Impollinatore[] impollinatori = {
        new Impollinatore("Api", "Soprattutto blu, giallo, viola", "Fresco, intenso", "Giorno", "Zigomorfa", "Nettare e/o polline"),
        new Impollinatore("Bombi", "Soprattutto blu, giallo, viola", "Fresco, intenso", "Giorno", "Zigomorfa", "Nettare e/o polline"),
        new Impollinatore("Coleotteri", "Verde o bianco", "Vario, intenso", "Giorno o notte", "Aperta", "Solo polline"),
        new Impollinatore("Farfalle diurne", "Brillante", "Fresco, delicato", "Giorno", "Piattaforma di atterraggio", "Solo nettare"),
        new Impollinatore("Farfalle notturne", "Spesso bianco o pallido", "Dolce, intenso", "Notte", "Profonda", "Solo nettare"),
        new Impollinatore("Mosche (Sirfidi)", "Luminoso", "Debole", "Giorno", "Attinomorfa", "Nettare e/o polline"),
        new Impollinatore("Mosche (Bombilidi)", "Indifferente", "Fresco, delicato", "Giorno", "Racchiusa", "Solo nettare")
    };
    Dictionary<int, int[]> fioriPerImpoll = new Dictionary<int, int[]>();
    //Farfalle
    static int[] fiori0 = { 0, 8, 3 }; //Margherita // Primula // Garofano
    //Api
    static int[] fiori1 = { 6, 4, 1 }; //Iris // Salvia // Ginestrino
    //Bombi
    static int[] fiori2 = { 11, 5, 7 }; // Pomodoro // Robinia // Linaria
    //Coleotteri
    static int[] fiori3 = { 10, 2, 14 }; //Rosa // Cisto // Euforbia 
    //Falene
    static int[] fiori4 = { 4, 12 }; //Salvia // Trifoglio
    //Sirfidi
    static int[] fiori5 = { 2, 9, 13 }; // Cisto // Malva // Sambuco
    //Bombilidi
    static int[] fiori6 = { 15, 4, 8 }; // Salvia // Veronica // Primula
    public SpriteRenderer[] spriteRenderersImp;
    public SpriteRenderer[] spriteRenderersFiori;
    public Sprite[] spriteArrayImp;
    public Sprite[] spriteArrayFiori;
    public TextMeshProUGUI[] NomeImp;
    public TextMeshProUGUI[] NomeFiori;
    string[] nomeFiori = { "Margherita", "Ginestrino", "Cisto", "Garofano", "Salvia", "Robinia", "Iris", "Linaria", "Primula",
        "Malva", "Rosa", "Pomodoro", "Trifoglio", "Sambuco", "Euforbia", "Veronica" };
    string[] nomeImp = { "Apis mellifera", "Bombus terrestris", "Cetonia aurata", "Oxythyrea funesta", "Aglais urticae", "Zerynthia", "Autographa gamma", "Syrphus ribesii", "Bombylius medius" };

    public static int indice = 0; //0...6


    public TextMeshProUGUI TitoloInfo;
    public TextMeshProUGUI SottotTitoloInfo1;
    public TextMeshProUGUI SottotTitoloInfo2;
    public TextMeshProUGUI infoText1;
    public TextMeshProUGUI infoText2;

    public SpriteRenderer[] spriteCorolla;
    public SpriteRenderer[] spriteBottom;
    public Sprite[] spriteArrayInfo;
    public GameObject scaleSprite;

    public static int indiceInfo = 0; //0...6

    private Info[] informazioni = {
        new Info("L'impollinazione", "Cos'e' l'impollinazione?", "",
            "Per impollinazione si intende il trasferimento del polline dalla parte maschile a quella femminile dei fiori. Si tratta di un processo fondamentale per la riproduzione di tutte le piante con semi. Una volta avvenuta la fecondazione, il fiore subisce delle trasformazioni che porteranno alla maturazione dei semi e alla formazione del frutto. Questo “servizio di trasporto” che e' l’impollinazione può essere svolto dal vento, dall’acqua o dagli animali.", ""),
        new Info("L'impollinazione", "L’importanza dell’impollinazione", "",
            "Oltre ad essere un processo indispensabile per la vita sulla Terra, l’impollinazione rappresenta un “beneficio ecosistemico” incredibilmente importante per l’uomo: l’agricoltura e la produzione di cibo sono strettamente dipendenti da questo processo naturale. Fino al 75% delle principali colture mondiali dipende dall’impollinazione animale.", ""),
        new Info("Gli impollinatori", "Chi sono?", "",
            "In tutto il mondo, gli insetti sono i piu' importanti ed efficienti impollinatori: api (Hymenoptera Apoidea), vespe (Hymenoptera Vespidae), mosche (Diptera), coleotteri (Coleoptera), farfalle e falene (Lepidoptera). Fra tutti questi, un ruolo particolarmente importante è rivestito dalle api selvatiche e dai sirfidi (un gruppo di mosche).", ""),
        new Info("Gli impollinatori", "Perche' sono in pericolo e vanno protetti?", "",
            "Dalla fine del ventesimo secolo, il calo delle popolazioni di insetti impollinatori e' stato documentato in tutto il mondo. La perdita di habitat, il cambiamento di uso del suolo, l’agricoltura intensiva, l’uso dei pesticidi e degli erbicidi, l’introduzione di specie invasive e il cambiamento climatico sono tra le maggiori cause del loro declino.", ""),
        new Info("Piante e impollinatori", "I motivi dell'attrazione", "",
            "Molte specie di piante vengono impollinate da insetti, per questo l’evoluzione ha favorito le caratteristiche piu adatte ad attirarli, come petali dai colori vivaci, fiori profumati e nettare abbondante. Piante e insetti ottengono reciproci vantaggi dall'impollinazione. Gli insetti trovano in nettare e polline importanti fonti di cibo e, spostandosi di fiore in fiore, trasportano involontariamente il polline permettendo la fecondazione.", ""),
        new Info("Forma corolla", "Zigomorfa", "Attinomorfa",
            "In botanica, si definisce “zigomorfo” un fiore con simmetria “a specchio”, che si può dividere in due meta' specularmente uguali lungo un unico piano di simmetria.",
            "Si definisce “attinomorfo” un fiore che presenta una simmetria raggiata, che puo' essere diviso lungo piu' piani di simmetria.")
    };

    // Start is called before the first frame update
    void Start()
    {
        fioriPerImpoll[3] = fiori0;

        fioriPerImpoll[0] = fiori1;

        fioriPerImpoll[1] = fiori2;
        fioriPerImpoll[2] = fiori3;
        fioriPerImpoll[4] = fiori4;
        fioriPerImpoll[5] = fiori5;
        fioriPerImpoll[6] = fiori6;
    }

    // Update is called once per frame
    void Update()
    {
        switch(StartGlossarioScript.paginaScelta)
        {
            case 0://Impollinatori
                PopolaImpollinatori();
                break;
            case 1://Info
                PopolaInfo();
                break;
            default: break;
        }
    }
    void PopolaImpollinatori()
    {
        TitoloImp.text = impollinatori[indice].Nome;
        colore.text = impollinatori[indice].Colore;
        profumo.text = impollinatori[indice].Profumo;
        periodo.text = impollinatori[indice].Periodo;
        corolla.text = impollinatori[indice].Corolla;
        ricompensa.text = impollinatori[indice].Ricompensa;

        switch (indice)
        {
            case 0: //ape
                spriteRenderersImp[0].sprite = spriteArrayImp[0];
                NomeImp[0].text = nomeImp[0];
                break;
            case 1: //bombo
                spriteRenderersImp[0].sprite = spriteArrayImp[1];
                NomeImp[0].text = nomeImp[1];
                break;
            case 2: //col
                spriteRenderersImp[0].sprite = spriteArrayImp[2];
                spriteRenderersImp[1].sprite = spriteArrayImp[3];
                NomeImp[0].text = nomeImp[2];
                NomeImp[1].text = nomeImp[3];
                break;
            case 3: //farf
                spriteRenderersImp[0].sprite = spriteArrayImp[4];
                spriteRenderersImp[1].sprite = spriteArrayImp[5];
                NomeImp[0].text = nomeImp[4];
                NomeImp[1].text = nomeImp[5];
                break;
            case 4: //farfN
                spriteRenderersImp[0].sprite = spriteArrayImp[6];
                NomeImp[0].text = nomeImp[6];
                break;
            case 5: //moscaR
                spriteRenderersImp[0].sprite = spriteArrayImp[7];
                NomeImp[0].text = nomeImp[7];
                break;
            case 6: //moscaI
                spriteRenderersImp[0].sprite = spriteArrayImp[8];
                NomeImp[0].text = nomeImp[8];
                break;
            default: break;
        }
        int[] fiori;
        fioriPerImpoll.TryGetValue(indice, out fiori);
        for (int i = 0; i < fiori.Length; i++)
        {
            spriteRenderersFiori[i].sprite = spriteArrayFiori[fiori[i]];
            NomeFiori[i].text = nomeFiori[fiori[i]];
        }
    }

    void PopolaInfo()
    {
        TitoloInfo.text = informazioni[indiceInfo].InfoTit;
        SottotTitoloInfo1.text = informazioni[indiceInfo].SottoTit1;
        infoText1.text = informazioni[indiceInfo].Info1;
        foreach (SpriteRenderer sprite in spriteCorolla)
        {
            sprite.enabled = false;
        }
        foreach (SpriteRenderer sprite in spriteBottom)
        {
            sprite.enabled = false;
        }
        /*spriteBottom[1].sprite = spriteArrayInfo[indiceInfo + 1];
        spriteBottom[0].sprite = spriteArrayInfo[indiceInfo];*/
        if (indiceInfo == 5)
        {
            SottotTitoloInfo2.text = informazioni[indiceInfo].SottoTit2;
            infoText2.text = informazioni[indiceInfo].Info2;
            foreach(SpriteRenderer sprite in spriteCorolla)
            {
                sprite.enabled = true;
            }
            foreach (SpriteRenderer sprite in spriteBottom)
            {
                sprite.enabled = false;
            }
        }
        scaleSprite.transform.localScale = indiceInfo == 5 ? new Vector3(15, 15, 0) : new Vector3(40, 40, 0);
    }
    private class Impollinatore
    {
        private string nome;
        private string colore;
        private string profumo;
        private string periodo;
        private string corolla;
        private string ricompensa;

        public Impollinatore(string nome, string colore, string profumo, string periodo, string corolla, string ricompensa)
        {
            this.nome = nome;
            this.colore = colore;
            this.profumo = profumo;
            this.periodo = periodo;
            this.corolla = corolla;
            this.ricompensa = ricompensa;
        }

        public string Nome
        {
            get { return nome; }
        }
        public string Colore
        {
            get { return colore; }
        }
        public string Profumo
        {
            get { return profumo; }
        }
        public string Periodo
        {
            get { return periodo; }
        }
        public string Corolla
        {
            get { return corolla; }
        }
        public string Ricompensa
        {
            get { return ricompensa; }
        }
    }

    private class Info
    {
        string infoTit;
        string sottoTit1;
        string sottoTit2;
        string info1;
        string info2;

        public Info(string infoTit, string sottoTit1, string sottoTit2, string info1, string info2)
        {
            this.infoTit = infoTit;
            this.sottoTit1 = sottoTit1;
            this.sottoTit2 = sottoTit2;
            this.info1 = info1;
            this.info2 = info2;
        }
        public string InfoTit
        {
            get { return infoTit; }
        }
        public string SottoTit1
        {
            get { return sottoTit1; }
        }
        public string SottoTit2
        {
            get { return sottoTit2; }
        }
        public string Info1
        {
            get { return info1; }
        }
        public string Info2
        {
            get { return info2; }
        }
    }
}
