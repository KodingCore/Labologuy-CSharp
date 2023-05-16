using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLoguy : MonoBehaviour {

	//Declaration des sprites
	public Sprite partTemoin;

	public List<GameObject> ImportParts;
	private GameObject newInstance;
	private GameObject textTouches;

	//Declaration des listes
	public List<GameObject> allParts;
	public List<string> partsNames;
	public List<Vector3> partsPositions;

	//Position en cours de traitement
	Vector3 partPositionRef;
	Quaternion partRotRef;

	//Resultat de la relation pixel d'une largeur par unité de mesure du moteur
	float largeurPieceTampon;
	int largeurPiece;

	//Definitions de x et de y selon largeurPiece
	int xPartPos;
	int yPartPos;

    //GameObject OK sur lequel je prélève le nombre de cases choisis par le changement de son nom
    private GameObject oK;
	//GameObject du menu à détruire après validation du nombres de cases	
	private GameObject howMuch;

	//string du nom de l'objet OK après sa modification (après validation du nombre de cases)
	string nouveauNomOK;
	//interger du nombre de cases obtenu pour un coté via le nouveau nom en string	
	int piecesCoteMax;
	//Definition de la position X ou Y maximum des pieces soit pieceCoteMax x 3
	public int longueurCoteMax;

	//Test de la destruction du menu
	bool testHideMenu = false;

	//Randoms lors de choix entre différentes pieces
	int numRandPart;
	int secNumRandPart;
	//Random qui vient casser le schema sur un coté
	int randBreakEasyer;
	string stadeChoisitBas;
	string stadeChoisitGauche;
	//Piece choisis pour etre instancié
	private GameObject partNow;
	//Nom de la piece en dessous
	string nameUnder;
	//Nom de la piece a gauche
	string nameLeft;
	//Numero du nom de la piece en dessous (0 ou 1)
	int numUnder;
	//Numero du dessous de la piece active (0 ou 1)
	int myNumUnder;
	//Numero du nom de la piece a gauche (0 ou 1)
	int numLeft;
	//Numero de gauche de la piece active (0 ou 1)
	int myNumLeft;
	//Contient le compte de pieces traité
	int pieceCounter;

	//Test de la fin de la création du lab
	bool create = false;

	//Test d'une premiere cassure sur le haut ou la droite pour placer une deuxieme piece coherente
	bool versPartNext = false;

	float orthographicSizeBasic = 0.0f;
	Vector3 transformCameraBasic = Vector3.zero;
	void Start() {
		textTouches = GameObject.Find("TextTouches");
		textTouches.SetActive(false);
		orthographicSizeBasic = Camera.main.orthographicSize;
		transformCameraBasic = transform.position;

		//Adaptation du script selon la largeur d'une piece
		largeurPieceTampon = partTemoin.textureRect.size.x / partTemoin.pixelsPerUnit;
		largeurPiece = Mathf.RoundToInt(largeurPieceTampon);

		//Initialisation des objets
		oK = GameObject.Find("OK");
		howMuch = GameObject.Find("HowMuch");
		partRotRef = this.transform.rotation;
	}

	void Update() {
		if (!testHideMenu)
		{ //Test de la destruction du menu (le choix du nombre de cases est fait)
			LaunchCrea();
		}
		else if (testHideMenu && !create)
		{
			Creation();
			while (AnalyseParts().Count > 0) // Analyse qui retourne une List<int> des innaccessibles (Tant qu'il y a des parties innaccessibles)--------------------------------------------------------------------
			{
				Reaccess(AnalyseParts()); //Réouvre les parties fermés -----> L'analyse se fait par AnalyseParts qui retourne une List<int> à Reaccess(), ce dernier ouvre les parties concernés
			}
			textTouches.SetActive(true);
		}
	}

	void LaunchCrea() {
		/*
		 Si le nom de l'objet oK est différent de son nom d'origine "OK"
		 nouveauNomOk prend son nouveau nom qui est en fait le nombre de cases d'un côté du Lab
		 piecesCoteMax integre la valeur du nom en la convertissant en Integer
		 longueurCoteMax est définie en multipliant largeurPiece par piecesCoteMax
		 On cache le menu
		 On passe testHideMenu sur true
		 
		 */
		if (oK.name != "OK") {
			nouveauNomOK = oK.name;
			piecesCoteMax = int.Parse(nouveauNomOK);
			longueurCoteMax = piecesCoteMax * largeurPiece;
			howMuch.SetActive(false);
			testHideMenu = true;
		}
	}

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
			//Application.Quit();
        }
		if(Input.GetKey(KeyCode.Space) && create && testHideMenu)
		{
			for(int i = 0; i < allParts.Count; i++)
            {
				Destroy(allParts[i]);
			}

			allParts.Clear();
			partsNames.Clear();
			partsPositions.Clear();
			howMuch.SetActive(true);
			testHideMenu = false;
			create = false;
			oK.name = "OK";
			Camera.main.orthographic = true;
            Camera.main.orthographicSize = orthographicSizeBasic;
			transform.position = transformCameraBasic;
			textTouches.SetActive(false);
		}
	}

    void Creation() {
		stadeChoisitBas = "NonFait";
		stadeChoisitGauche = "NonFait";
		for (int y = 0; y < piecesCoteMax; y++)
		{ //axe y (tant que y est inférieur à XYmax)
			for (int x = 0; x < piecesCoteMax; x++)
			{ //axe x (tant que x est inférieur à XYmax)
				xPartPos = x * largeurPiece; //Poisition X de la nouvelle pièce
				yPartPos = y * largeurPiece; //Position Y de la nouvelle pièce
				partPositionRef = new Vector3(xPartPos, yPartPos, 5);
				partsPositions.Add(partPositionRef);
				pieceCounter = x + y * piecesCoteMax;
				if (y == 0)
				{ //ligne du bas
					if (x == 0)
					{ //case en bas a gauche
						partNow = ImportParts[6];
					}
					else if (x > 0 && x < piecesCoteMax - 1)
					{ //cases de l'interieur du bas

						if (stadeChoisitBas == "NonFait" && x < piecesCoteMax - 2)
						{
							randBreakEasyer = Random.Range(0, 10);
							if (randBreakEasyer < 4)
							{
								stadeChoisitBas = "Commencer";
							}
						}
						if (stadeChoisitBas == "Commencer" || stadeChoisitBas == "Continuer")
						{
							if (stadeChoisitBas == "Commencer")
                            {
								randBreakEasyer = Random.Range(0, 2);
								if (randBreakEasyer == 0)
                                {
									partNow = ImportParts[11]; //1100
								}
                                else
                                {
									partNow = ImportParts[7]; //1000
								}
								stadeChoisitBas = "Continuer";
							}
							else if (stadeChoisitBas == "Continuer")
							{
								randBreakEasyer = Random.Range(0, 2);
								if (randBreakEasyer == 0)
								{
									partNow = ImportParts[5]; //0110
								}
								else
								{
									partNow = ImportParts[1]; //0010
								}
								stadeChoisitBas = "NonFait";
							}
						}
						else
						{
							numRandPart = Random.Range(0, 2); //Random entre 1010 1110
							if (numRandPart == 1)
							{
								partNow = ImportParts[9]; //1010
							}
							else
							{
								partNow = ImportParts[13]; //1110
							}
						}
					}
					else if (x == piecesCoteMax - 1)
					{ //case en bas a droite
						partNow = ImportParts[11]; //1100
					}
				}
				else if (y > 0 && y < piecesCoteMax - 1)
				{ //lignes interieur
					if (x == 0)
					{ //cases de l'interieur de gauche
						if (stadeChoisitGauche == "NonFait" && y < piecesCoteMax - 2)
						{
							randBreakEasyer = Random.Range(0, 10);
							if (randBreakEasyer < 4)
							{
								stadeChoisitGauche = "Commencer";
							}
						}
						if (stadeChoisitGauche == "Commencer" || stadeChoisitGauche == "Continuer")
						{
							if (stadeChoisitGauche == "Commencer")
							{
								randBreakEasyer = Random.Range(0, 2);
								if (randBreakEasyer == 0)
								{
									partNow = ImportParts[2]; //0011
								}
								else
								{
									partNow = ImportParts[0]; //0001
								}
								stadeChoisitGauche = "Continuer";
							}
							else if (stadeChoisitGauche == "Continuer")
							{
								randBreakEasyer = Random.Range(0, 2);
								if (randBreakEasyer == 0)
								{
									partNow = ImportParts[5]; //0110
								}
								else
								{
									partNow = ImportParts[3]; //0100
								}
								stadeChoisitGauche = "NonFait";
							}
						}
						else
						{
							numRandPart = Random.Range(0, 2); //Random entre 0101 et 0111
							if (numRandPart == 1)
							{
								partNow = ImportParts[4]; //0101
							}
							else
							{
								partNow = ImportParts[6]; //0111
							}
						}

					}
					else if (x > 0 && x < piecesCoteMax - 1)
					{ //cases de l'interieur
						numUnder = pieceCounter - piecesCoteMax;
						nameUnder = partsNames[numUnder];
						numLeft = pieceCounter - 1;
						nameLeft = partsNames[numLeft];
						numUnder = nameUnder[1];
						numUnder -= 48;
						numLeft = nameLeft[2];
						numLeft -= 48;
						do
						{
							numRandPart = Random.Range(0, 14); //Random entre toutes les pieces (15 pour ajouter la croix)
							partNow = ImportParts[numRandPart];
							myNumUnder = partNow.name[3];
							myNumUnder -= 48;
							myNumLeft = partNow.name[0];
							myNumLeft -= 48;
							if (partNow.name == "1000" || partNow.name == "0100" || partNow.name == "0001" || partNow.name == "0010" || partNow.name == "1111")
							{
								secNumRandPart = Random.Range(0, 11);
								if (secNumRandPart <= 5)
								{
									myNumLeft = -1;
								}
							}
						} while (myNumUnder != numUnder || myNumLeft != numLeft || myNumLeft == -1);/*<<<piece retirée du jeux pour éviter les boucles*/
					}
					else if (x == piecesCoteMax - 1)
					{ //cases de l'interieur de droite
						numLeft = pieceCounter - 1;
						nameLeft = partsNames[numLeft];
						numLeft = nameLeft[2];
						numLeft -= 48;
						if (numLeft == 0)
						{
							partNow = ImportParts[4];
						}
						else
						{
							partNow = ImportParts[12];
						}
						if(y < piecesCoteMax - 3 || versPartNext)
                        {
							randBreakEasyer = Random.Range(0, 8);
							if (versPartNext)
							{
								if (numLeft == 0) // Si la gauche est fermée
								{
									partNow = ImportParts[3];//0100
								}
								else// Si la gauche est ouverte
								{
									partNow = ImportParts[11];//1100
								}
								versPartNext = false;
							}
							else if (randBreakEasyer < 3)
							{
								if (numLeft == 0) // Si la gauche est fermée
								{
									partNow = ImportParts[0];//0001 pour la premiere
									versPartNext = true;
								}
								else// Si la gauche est ouverte
								{
									partNow = ImportParts[8];//1001 pour la premiere
									versPartNext = true;
								}
							}
						}
					}
				}
				else if (y == piecesCoteMax - 1)
				{ //ligne du haut

					if (x == 0)
					{ //case en haut a gauche
						partNow = ImportParts[2];
						versPartNext = false;
					}
					else if (x > 0 && x < piecesCoteMax - 1)
					{ //cases de l'interieur du haut
						numUnder = pieceCounter - piecesCoteMax;
						nameUnder = partsNames[numUnder];
						numUnder = nameUnder[1];
						numUnder -= 48;
						if (numUnder == 0)// Si le dessous est fermé
						{
							partNow = ImportParts[9];//1010
						}
						else// Si le dessous est ouvert
						{
							partNow = ImportParts[10];//1011
						}
						if (x < piecesCoteMax - 3 || versPartNext)
						{
							randBreakEasyer = Random.Range(0, 8);
							if (versPartNext)
							{
								if (numUnder == 0) // Si le dessous est fermé
								{
									partNow = ImportParts[1];//0010
								}
								else// Si le dessous est ouvert
								{
									partNow = ImportParts[2];//0011
								}
								versPartNext = false;
							}
							else if (randBreakEasyer < 3)
							{
								if (numUnder == 0) // Si le dessous est fermé
								{
									partNow = ImportParts[7];//1000 pour la premiere
									versPartNext = true;
								}
								else// Si le dessous est ouvert
								{
									partNow = ImportParts[8];//1001 pour la premiere
									versPartNext = true;
								}
                            }
						}
					}
					else if (x == piecesCoteMax - 1)
					{ //case en haut a droite
						partNow = ImportParts[12];
					}
				}
				newInstance = Instantiate(partNow, partPositionRef, partRotRef);
				newInstance.name = newInstance.name[0] + "" + newInstance.name[1] + "" + newInstance.name[2] + "" + newInstance.name[3];
				partsNames.Add(newInstance.name);
				partsPositions.Add(partPositionRef);
                allParts.Add(newInstance);
			}
		}
		//Ajuster la camera
		//Camera.main.orthographicSize = piecesCoteMax * 2;-20 pour 8 -40 pour 16 -84 pour 32 -170 pour 64
		Camera.main.orthographic = false;
		float zInteger = 0;
		if(piecesCoteMax == 8)
		{
			zInteger = -32;
        }
        else if(piecesCoteMax == 16){
			zInteger = -64;
		}else if(piecesCoteMax == 32)
		{
			zInteger = -128;
		}
		transform.position = new Vector3(longueurCoteMax / 2 - largeurPiece / 2, longueurCoteMax / 2 - largeurPiece / 2, zInteger);
		//bool create
		create = true;
	}

	
	List<int> AnalyseParts()
	{
		List<int> partForAnalyse = new();
		List<int> partsTamponForAnalyse = new();
		List<int> partsInnaccessible = new();
		partsTamponForAnalyse.Add(0);
		partForAnalyse.Add(0);
		int i = 0;
		for(int tampon = 0; partsTamponForAnalyse.Count < allParts.Count; tampon++)
        {
			if (partsTamponForAnalyse.Count - 1 >= tampon)
			{
				if (partsNames[partsTamponForAnalyse[tampon]][0] == '1')
				{
					if (!partsTamponForAnalyse.Contains(partsTamponForAnalyse[tampon] - 1))
					{
						partForAnalyse.Add(partsTamponForAnalyse[tampon] - 1);
					}
				}
				if (partsNames[partsTamponForAnalyse[tampon]][1] == '1' && partsTamponForAnalyse[tampon] != piecesCoteMax * piecesCoteMax - 1)
				{
					if (!partsTamponForAnalyse.Contains(partsTamponForAnalyse[tampon] + piecesCoteMax))
					{
						partForAnalyse.Add(partsTamponForAnalyse[tampon] + piecesCoteMax);
					}
				}
				if (partsNames[partsTamponForAnalyse[tampon]][2] == '1')
				{
					if (!partsTamponForAnalyse.Contains(partsTamponForAnalyse[tampon] + 1))
					{
						partForAnalyse.Add(partsTamponForAnalyse[tampon] + 1);
					}
				}
				if (partsNames[partsTamponForAnalyse[tampon]][3] == '1' && partsTamponForAnalyse[tampon] != 0)
				{
					if (!partsTamponForAnalyse.Contains(partsTamponForAnalyse[tampon] - piecesCoteMax))
					{
						partForAnalyse.Add(partsTamponForAnalyse[tampon] - piecesCoteMax);
					}
				}
				partsTamponForAnalyse = partForAnalyse;
			}
			if(i > allParts.Count)
			{
				break;
            }
			i++;
		}
		bool contin;
		for(int num=0; num < allParts.Count-1;num++) //Boucle for pour integrer le numero des pieces innaccessibles dans partsInnaccessible
		{
			contin = false;
			foreach (int numer in partsTamponForAnalyse)
            {
				if (num == numer)
                {
					contin = true;
				}
            }
			if(contin == false)
            {
				partsInnaccessible.Add(num);
			}
        }
		Debug.Log(partsInnaccessible.Count);
		return partsInnaccessible;
	}

	void Reaccess(List<int> innaccessList)
    {
		GameObject partieTraitee;
		GameObject partieCollegue;

		GameObject nouvellePartiePrincipale = null;
		GameObject nouvellePartieCollegue = null;
		
		int numCollegue;

		string nomOriginalTraitee;
		string nomOriginalCollegue;

		if (innaccessList.Count > 0)
        {
			partieTraitee = allParts[innaccessList[0]];
            nomOriginalTraitee = partieTraitee.name;
			if (innaccessList[0] % piecesCoteMax == 0) //C'est une piece de la premiere colonne
            {
				nomOriginalTraitee = "" + nomOriginalTraitee[0] + nomOriginalTraitee[1] + nomOriginalTraitee[2] + "1";
				Debug.Log("" + nomOriginalTraitee + "  " + innaccessList[0]);
				numCollegue = innaccessList[0] - piecesCoteMax;
				partieCollegue = allParts[numCollegue];
				nomOriginalCollegue = partieCollegue.name;
				nomOriginalCollegue = "" + nomOriginalCollegue[0] + "1" + nomOriginalCollegue[2] + nomOriginalCollegue[3];
			}
            else
            {
				nomOriginalTraitee = "1"+nomOriginalTraitee[1]+nomOriginalTraitee[2]+nomOriginalTraitee[3];

				numCollegue = innaccessList[0] - 1;
				partieCollegue = allParts[numCollegue];
				nomOriginalCollegue = partieCollegue.name;
				nomOriginalCollegue = "" + nomOriginalCollegue[0] + nomOriginalCollegue[1] + "1" + nomOriginalCollegue[3];
			}

			foreach (GameObject partsImportees in ImportParts)
			{

				if (partsImportees.name == nomOriginalTraitee)
				{
					nouvellePartiePrincipale = partsImportees;
				}

				if (partsImportees.name == nomOriginalCollegue)
				{
					nouvellePartieCollegue = partsImportees;

				}
			}

			//PARTIE PRINCIPALE

			partPositionRef = allParts[innaccessList[0]].transform.position;
			partRotRef = allParts[innaccessList[0]].transform.rotation;
			newInstance = Instantiate(nouvellePartiePrincipale, partPositionRef, partRotRef);
			newInstance.name = newInstance.name[0] + "" + newInstance.name[1] + "" + newInstance.name[2] + "" + newInstance.name[3];
			partsNames[innaccessList[0]] = newInstance.name;
			partsPositions[innaccessList[0]] = partPositionRef;
			allParts[innaccessList[0]] = newInstance;

			// PARTIE COLLEGUE
			 
			partPositionRef = partieCollegue.transform.position;
			partRotRef = partieCollegue.transform.rotation;
			newInstance = Instantiate(nouvellePartieCollegue, partPositionRef, partRotRef);
			newInstance.name = newInstance.name[0] + "" + newInstance.name[1] + "" + newInstance.name[2] + "" + newInstance.name[3];
			partsNames[numCollegue] = newInstance.name;
			partsPositions[numCollegue] = partPositionRef;
			allParts[numCollegue] = newInstance;

			Destroy(partieTraitee);
			Destroy(partieCollegue);
			
		}
    }
}
