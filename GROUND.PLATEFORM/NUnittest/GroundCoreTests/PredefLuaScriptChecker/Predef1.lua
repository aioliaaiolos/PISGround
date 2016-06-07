dofile("F:/PIS Files/PIS Base/Scripts/Background/PredefSync.lua");

function _Initialization()  
  
  OVERRIDE_ALL = 8;
  AUDIO_1	   = 9;
  gFraPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."fra/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."fra/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."fra/Message.php?pText=",
    [evtmgr.MISSION_NOT_INIT] = mission_pack.WEB_ROOT.."fra/NI.php",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
  
  gEngPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."eng/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."eng/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."eng/Message.php?pText=",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
  
  gGerPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."ger/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."ger/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."ger/Message.php?pText=",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
  
  gItaPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."ita/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."ita/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."ita/Message.php?pText=",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
  
  gSpaPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."spa/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."spa/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."spa/Message.php?pText=",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
  
  gCatPredefWebPages = 
  {
    ["100"] = mission_pack.WEB_ROOT.."cat/Taxi.php",
    ["101"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["102"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["103"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["104"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["105"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["106"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["107"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["108"] = mission_pack.WEB_ROOT.."cat/MessageLibre.php?pText=",
    ["109"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["110"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["111"] = mission_pack.WEB_ROOT.."cat/Message.php?pText=",
    ["VariableMonitor"] = mission_pack.WEB_ROOT.."fra/VarMonitor.php?variables=",
  }
 
  --Création d'un client pour le service de dictionnaire actif dans l'élément
  dic = dictionary.create();
  
  -- Connexion au dictionnaire des variables du système SIV
  if (dictionary.connect(dic, dictionary.TRAIN_LOCAL, nil)) then
  
   
    evtmgr.init(dic)
    
    evtmgr.add_varargs( "Mission.TrainNumber" );
    evtmgr.add_varargs( "Mission.Type" );
    evtmgr.add_varargs( "Mission.FirstStation.ShortName[fra]" );
    evtmgr.add_varargs( "Mission.LastStation.ShortName[fra]" );
    evtmgr.add_varargs( "Mission.RemainInterStationList.StationNames[fra]" );
    evtmgr.add_varargs( "Mission.InterStationList.StationNames[fra]" );
    evtmgr.add_varargs( "SIV-AUTO-INHIB-ETAT" );
    
    
    
    predef.init(dic);
     
    util.trace( "predef.init done", util.L5 );     
    
    pdefTaxi = predef.create_instant_msg("100", OnTaxiAnnounce);
    predef.set_class(pdefTaxi, "INFO");
    predef.set_description(pdefTaxi, "fra", "Annonce Taxi");
    predef.set_description(pdefTaxi, "eng", "[eng] Annonce Taxi");
    predef.set_description(pdefTaxi, "ger", "[ger] Annonce Taxi");
	predef.set_description(pdefTaxi, "ita", "[ita] Annonce Taxi");
	predef.set_description(pdefTaxi, "spa", "[spa] Annonce Taxi");
    predef.set_description(pdefTaxi, "cat", "[cat] Annonce Taxi");
    
    pdefDoctor = predef.create_instant_msg("101", OnDoctorAnnounce);
    predef.set_class(pdefDoctor, "PRIORITAIRE");
    predef.set_description(pdefDoctor, "fra", "Annonce demande médecin en voiture");
    predef.set_description(pdefDoctor, "eng", "[eng] Annonce demande médecin en voiture");
    predef.set_description(pdefDoctor, "ger", "[ger] Annonce demande médecin en voiture");
	predef.set_description(pdefDoctor, "ita", "[ita] Annonce demande médecin en voiture");
	predef.set_description(pdefDoctor, "spa", "[spa] Annonce demande médecin en voiture");
	predef.set_description(pdefDoctor, "cat", "[cat] Annonce demande médecin en voiture");
    predef.add_input_param(pdefDoctor, predef.CARID_PARAM);
    
    pdefDelay = predef.create_instant_msg("102", OnDelayAnnounce);
    predef.set_class(pdefDelay, "INFO");
    predef.set_description(pdefDelay, "fra", "Annonce retard exceptionnel a l'arrivée en Gare");
    predef.set_description(pdefDelay, "eng", "[eng] Annonce retard exceptionnel a l'arrivée en Gare");
    predef.set_description(pdefDelay, "ger", "[ger] Annonce retard exceptionnel a l'arrivée en Gare");
	predef.set_description(pdefDelay, "ita", "[ita] Annonce retard exceptionnel a l'arrivée en Gare");
	predef.set_description(pdefDelay, "spa", "[spa] Annonce retard exceptionnel a l'arrivée en Gare");
	predef.set_description(pdefDelay, "cat", "[cat] Annonce retard exceptionnel a l'arrivée en Gare");
    predef.add_input_param(pdefDelay, predef.DELAY_PARAM);
    predef.add_input_param(pdefDelay, predef.STATION_PARAM);
    
    pDelayMsg = predef.create_instant_msg("103", OnEstimatedDelayAnnounce)
    predef.set_class(pDelayMsg, "INFO");
    predef.set_description(pDelayMsg, "fra", "Retard a l'arrivee estime a...");
    predef.set_description(pDelayMsg, "eng", "[eng] Retard a l'arrivee estime a...");
    predef.set_description(pDelayMsg, "ger", "[ger] Retard a l'arrivee estime a...");
	predef.set_description(pDelayMsg, "ita", "[ita] Retard a l'arrivee estime a...");
	predef.set_description(pDelayMsg, "spa", "[spa] Retard a l'arrivee estime a...");
	predef.set_description(pDelayMsg, "cat", "[cat] Retard a l'arrivee estime a...");
    predef.add_input_param(pDelayMsg, predef.DELAY_PARAM);

    pDelayMsg = predef.create_instant_msg("104", OnDelayWeatherAnnounce)
    predef.set_class(pDelayMsg, "INFO");
    predef.set_description(pDelayMsg, "fra", "Retard METEO estime a ...");
    predef.set_description(pDelayMsg, "eng", "[eng] Retard METEO...");
    predef.set_description(pDelayMsg, "ger", "[ger] Retard METEO...");
	predef.set_description(pDelayMsg, "ita", "[ita] Retard METEO...");
	predef.set_description(pDelayMsg, "spa", "[spa] Retard METEO...");
	predef.set_description(pDelayMsg, "cat", "[cat] Retard METEO...");
    predef.add_input_param(pDelayMsg, predef.DELAY_PARAM);
    
    pdefBar = predef.create_instant_msg("105", OnBarAnnounce);
    predef.set_class(pdefBar, "INFO");
    predef.set_description(pdefBar, "fra", "Un Bar en voiture...");
    predef.set_description(pdefBar, "eng", "[eng] Un Bar en voiture...");
    predef.set_description(pdefBar, "ger", "[ger] Un Bar en voiture...");
	predef.set_description(pdefBar, "ita", "[ita] Un Bar en voiture...");
	predef.set_description(pdefBar, "spa", "[spa] Un Bar en voiture...");
	predef.set_description(pdefBar, "cat", "[cat] Un Bar en voiture...");
    predef.add_input_param(pdefBar, predef.CARID_PARAM);
    
    pdefUFR = predef.create_instant_msg("106", OnUFRAnnounce);
    predef.set_class(pdefUFR, "INFO");
    predef.set_description(pdefUFR, "fra", "Annonce accueil disponible pour fauteuils roulants");
    predef.set_description(pdefUFR, "eng", "[eng] Annonce accueil disponible pour fauteuils roulants");
    predef.set_description(pdefUFR, "ger", "[ger] Annonce accueil disponible pour fauteuils roulants");
	predef.set_description(pdefUFR, "ita", "[ita] Annonce accueil disponible pour fauteuils roulants");
    predef.set_description(pdefUFR, "spa", "[spa] Annonce accueil disponible pour fauteuils roulants");
    predef.set_description(pdefUFR, "cat", "[cat] Annonce accueil disponible pour fauteuils roulants");
    predef.add_input_param(pdefUFR, predef.STATION_PARAM);
    
    pdefDoorsLocked = predef.create_instant_msg("107", OnDoorsLockedAnnounce);
    predef.set_class(pdefDoorsLocked, "PRIORITAIRE");
    predef.set_description(pdefDoorsLocked, "fra", "Annonce fermeture des portes");
    predef.set_description(pdefDoorsLocked, "eng", "[eng] Annonce fermeture des portes");
    predef.set_description(pdefDoorsLocked, "ger", "[ger] Annonce fermeture des portes");
	predef.set_description(pdefDoorsLocked, "ita", "[ita] Annonce fermeture des portes");
    predef.set_description(pdefDoorsLocked, "spa", "[spa] Annonce fermeture des portes");
    predef.set_description(pdefDoorsLocked, "cat", "[cat] Annonce fermeture des portes");
    
    freeMsg = predef.create_free_msg("108", OnFreeText, OnAttention);
    predef.set_class(freeMsg, "INFO");
    predef.set_description(freeMsg, "fra", "Message texte libre en Francais");
    predef.set_description(freeMsg, "eng", "[eng] Message texte libre en Francais");
    predef.set_description(freeMsg, "ger", "[ger] Message texte libre en Francais");
	predef.set_description(freeMsg, "ita", "[ita] Message texte libre en Francais");
    predef.set_description(freeMsg, "spa", "[spa] Message texte libre en Francais");
    predef.set_description(freeMsg, "cat", "[cat] Message texte libre en Francais");
  
    autoDelay = predef.create_auto_delay_msg("109", OnAutoDelay);
    predef.set_class(autoDelay, "INFO");
    predef.set_description(autoDelay, "fra", "Message de Retard automatique");
    predef.set_description(autoDelay, "eng", "[eng] Message de Retard automatique");
    predef.set_description(autoDelay, "ger", "[ger] Message de Retard automatique");  
	predef.set_description(autoDelay, "ita", "[ita] Message de Retard automatique");
    predef.set_description(autoDelay, "spa", "[spa] Message de Retard automatique");
    predef.set_description(autoDelay, "cat", "[cat] Message de Retard automatique");  
    
    autoDelay1 = predef.create_auto_delay_msg("111", OnAutoDelay1);
    predef.set_class(autoDelay1, "INFO");
    predef.set_description(autoDelay1, "fra", "Retard avec nom de gare");
    predef.set_description(autoDelay1, "eng", "[eng] Retard avec nom de gare");
    predef.set_description(autoDelay1, "ger", "[ger] Retard avec nom de gare");      
	predef.set_description(autoDelay1, "ita", "[ita] Retard avec nom de gare");
    predef.set_description(autoDelay1, "spa", "[spa] Retard avec nom de gare");
    predef.set_description(autoDelay1, "cat", "[cat] Retard avec nom de gare");  
     
    pdef3param = predef.create_instant_msg("110", OnDoorBlocked);
    predef.set_class(pdef3param, "INFO");
    predef.set_description(pdef3param, "fra", "Une porte bloquée en voiture...");
    predef.set_description(pdef3param, "eng", "[eng] Une porte bloquée en voiture...");
    predef.set_description(pdef3param, "ger", "[ger] Une porte bloquée en voiture...");
	predef.set_description(pdef3param, "ita", "[ita] Une porte bloquée en voiture...");
    predef.set_description(pdef3param, "spa", "[spa] Une porte bloquée en voiture...");
    predef.set_description(pdef3param, "cat", "[cat] Une porte bloquée en voiture...");
    predef.add_input_param(pdef3param, predef.CARID_PARAM);
    predef.add_input_param(pdef3param, predef.DELAY_PARAM);
    predef.add_input_param(pdef3param, predef.STATION_PARAM);
	
	pvarTestSonoDiff = predef.create_instant_msg("10A", OnTestSonoDiff, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarTestSonoDiff, "PRIORITAIRE");
    predef.set_description(pvarTestSonoDiff, "fra", "Test Sono Diffusion");
	predef.set_description(pvarTestSonoDiff, "eng", "[eng] Test Sono Diffusion");
    predef.set_description(pvarTestSonoDiff, "ger", "[ger] Test Sono Diffusion");
	predef.set_description(pvarTestSonoDiff, "ita", "[ita] Test Sono Diffusion");
    predef.set_description(pvarTestSonoDiff, "spa", "[spa] Test Sono Diffusion");
    predef.set_description(pvarTestSonoDiff, "cat", "[cat] Test Sono Diffusion");
	predef.add_input_param(pvarTestSonoDiff, predef.CARID_PARAM);

    pvarMonRetard = predef.create_instant_msg("200", OnVarMonRetard, evtmgr.LANGUAGE_LIST_BY_CALL );
    predef.set_class(pvarMonRetard, "INFO");
    predef.set_description(pvarMonRetard, "fra", "Moniteur de retard");
    predef.set_description(pvarMonRetard, "eng", "[eng] Moniteur de retard");
    predef.set_description(pvarMonRetard, "ger", "[ger] Moniteur de retard");
	predef.set_description(pvarMonRetard, "ita", "[ita] Moniteur de retard");
    predef.set_description(pvarMonRetard, "spa", "[spa] Moniteur de retard");
    predef.set_description(pvarMonRetard, "cat", "[cat] Moniteur de retard");
    
    pvarMonLum = predef.create_instant_msg("201", OnVarMonLum, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonLum, "INFO");
    predef.set_description(pvarMonLum, "fra", "Moniteur de Luminosité");
    predef.set_description(pvarMonLum, "eng", "[eng] Moniteur de Luminosit");
    predef.set_description(pvarMonLum, "ger", "[ger] Moniteur de Luminosit");
	predef.set_description(pvarMonLum, "ita", "[ita] Moniteur de Luminosité");
    predef.set_description(pvarMonLum, "spa", "[spa] Moniteur de Luminosit");
    predef.set_description(pvarMonLum, "cat", "[cat] Moniteur de Luminosit");
    
    pvarMonStatut = predef.create_instant_msg("202", OnVarMonStatut, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonStatut, "INFO");
    predef.set_description(pvarMonStatut, "fra", "Moniteur de statut");
    predef.set_description(pvarMonStatut, "eng", "[eng] Moniteur de statut");
    predef.set_description(pvarMonStatut, "ger", "[ger] Moniteur de statut");
	predef.set_description(pvarMonStatut, "ita", "[ita] Moniteur de statut");
    predef.set_description(pvarMonStatut, "spa", "[spa] Moniteur de statut");
    predef.set_description(pvarMonStatut, "cat", "[cat] Moniteur de statut");

    pvarMonSante1 = predef.create_instant_msg("204", OnVarMonSante1, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonSante1, "INFO");
    predef.set_description(pvarMonSante1, "fra", "Moniteur de Santé1");
    predef.set_description(pvarMonSante1, "eng", "[eng] Moniteur de Santé1");
    predef.set_description(pvarMonSante1, "ger", "[ger] Moniteur de Santé1");
	predef.set_description(pvarMonSante1, "ita", "[ita] Moniteur de Santé1");
    predef.set_description(pvarMonSante1, "spa", "[spa] Moniteur de Santé1");
    predef.set_description(pvarMonSante1, "cat", "[cat] Moniteur de Santé1");
    
    pvarMonSante2 = predef.create_instant_msg("205", OnVarMonSante2, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonSante2, "INFO");
    predef.set_description(pvarMonSante2, "fra", "Moniteur de Santé2");
    predef.set_description(pvarMonSante2, "eng", "[eng] Moniteur de Santé2");
    predef.set_description(pvarMonSante2, "ger", "[ger] Moniteur de Santé2");
	predef.set_description(pvarMonSante2, "ita", "[ita] Moniteur de Santé2");
    predef.set_description(pvarMonSante2, "spa", "[spa] Moniteur de Santé2");
    predef.set_description(pvarMonSante2, "cat", "[cat] Moniteur de Santé2");
    
    pvarMonDateHeure = predef.create_instant_msg("206", OnVarMonDateHeure, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonDateHeure, "INFO");
    predef.set_description(pvarMonDateHeure, "fra", "Moniteur de Date et H");
    predef.set_description(pvarMonDateHeure, "eng", "[eng] Moniteur de Date et H");
    predef.set_description(pvarMonDateHeure, "ger", "[ger] Moniteur de Date et H");
	predef.set_description(pvarMonDateHeure, "ita", "[ita] Moniteur de Date et H");
    predef.set_description(pvarMonDateHeure, "spa", "[spa] Moniteur de Date et H");
    predef.set_description(pvarMonDateHeure, "cat", "[cat] Moniteur de Date et H");
    
    pvarMonVersion1 = predef.create_instant_msg("207", OnVarMonVersion1, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonVersion1, "INFO");
    predef.set_description(pvarMonVersion1, "fra", "Moniteur de Version IHM");
    predef.set_description(pvarMonVersion1, "eng", "[eng] Moniteur de Version IHM");
    predef.set_description(pvarMonVersion1, "ger", "[ger] Moniteur de Version IHM");
	predef.set_description(pvarMonVersion1, "ita", "[ita] Moniteur de Version IHM");
    predef.set_description(pvarMonVersion1, "spa", "[spa] Moniteur de Version IHM");
    predef.set_description(pvarMonVersion1, "cat", "[cat] Moniteur de Version IHM");
    
    pvarMonVersion2 = predef.create_instant_msg("208", OnVarMonVersion2, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonVersion2, "INFO");
    predef.set_description(pvarMonVersion2, "fra", "Moniteur de Version");
    predef.set_description(pvarMonVersion2, "eng", "[eng] Moniteur de Version");
    predef.set_description(pvarMonVersion2, "ger", "[ger] Moniteur de Version");
	predef.set_description(pvarMonVersion2, "ita", "[ita] Moniteur de Version");
    predef.set_description(pvarMonVersion2, "spa", "[spa] Moniteur de Version");
    predef.set_description(pvarMonVersion2, "cat", "[cat] Moniteur de Version");
    
    pvarMonGPS = predef.create_instant_msg("209", OnVarMonGPS, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonGPS, "INFO");
    predef.set_description(pvarMonGPS, "fra", "Moniteur de GPS");
    predef.set_description(pvarMonGPS, "eng", "[eng] Moniteur de GPS");
    predef.set_description(pvarMonGPS, "ger", "[ger] Moniteur de GPS");
	predef.set_description(pvarMonGPS, "ita", "[ita] Moniteur de GPS");
    predef.set_description(pvarMonGPS, "spa", "[spa] Moniteur de GPS");
    predef.set_description(pvarMonGPS, "cat", "[cat] Moniteur de GPS");
	
--[[  
    pvarMonLMT = predef.create_instant_msg("210", OnVarMonLMT, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonLMT, "INFO");
    predef.set_description(pvarMonLMT, "fra", "Moniteur de LMT");
    predef.set_description(pvarMonLMT, "eng", "Moniteur de LMT");
    predef.set_description(pvarMonLMT, "ger", "Moniteur de LMT");
 --]]   
    pvarMonCouplage = predef.create_instant_msg("211", OnVarMonCouplage, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonCouplage, "INFO");
    predef.set_description(pvarMonCouplage, "fra", "Moniteur de Couplage");
    predef.set_description(pvarMonCouplage, "eng", "[eng] Moniteur de Couplage");
    predef.set_description(pvarMonCouplage, "ger", "[ger] Moniteur de Couplage");
	predef.set_description(pvarMonCouplage, "ita", "[ita] Moniteur de Couplage");
    predef.set_description(pvarMonCouplage, "spa", "[spa] Moniteur de Couplage");
    predef.set_description(pvarMonCouplage, "cat", "[cat] Moniteur de Couplage");
    
    
    pvarMP3Play = predef.create_instant_msg("212", OnPlayMP3, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMP3Play, "INFO");
    predef.set_description(pvarMP3Play, "fra", "Play MP3");
    
    pvarPinkNoiseMP3Play = predef.create_instant_msg("213", OnPlayPinkNoiseMP3, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarPinkNoiseMP3Play, "INFO");
    predef.set_description(pvarPinkNoiseMP3Play, "fra", "Play Pink Noise MP3");
    
    pvarMP3Pause = predef.create_instant_msg("214", OnPauseMP3, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMP3Pause, "INFO");
    predef.set_description(pvarMP3Pause, "fra", "Pause MP3");
    
    pvarMP3Stop = predef.create_instant_msg("215", OnStopMP3, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMP3Stop, "INFO");
    predef.set_description(pvarMP3Stop, "fra", "Stop MP3");
    
    paudioOnly = predef.create_instant_msg("216", OnAudioOnly, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(paudioOnly, "INFO");
    predef.set_description(paudioOnly, "fra", "Audio Only");
    predef.set_description(paudioOnly, "eng", "[eng] Audio Only");
    predef.set_description(paudioOnly, "ger", "[ger] Audio Only");
	predef.set_description(paudioOnly, "ita", "[ita] Audio Only");
    predef.set_description(paudioOnly, "spa", "[spa] Audio Only");
    predef.set_description(paudioOnly, "cat", "[cat] Audio Only");
    
    pSetGong = predef.create_instant_msg("217", OnSetGong, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pSetGong, "INFO");
    predef.set_description(pSetGong, "fra", "Set Gong");
    predef.set_description(pSetGong, "eng", "[eng] Set Gong");
    predef.set_description(pSetGong, "ger", "[ger] Set Gong");
	predef.set_description(pSetGong, "ita", "[ita] Set Gong");
    predef.set_description(pSetGong, "spa", "[spa] Set Gong");
    predef.set_description(pSetGong, "cat", "[cat] Set Gong");
    predef.add_input_param(pSetGong, predef.CARID_PARAM);
    predef.add_input_param(pSetGong, predef.DELAY_PARAM);
    
    pvarMonRecalage1 = predef.create_instant_msg("218", OnVarMonRecalage1, evtmgr.LANGUAGE_LIST_BY_CALL);
    predef.set_class(pvarMonRecalage1, "INFO");
    predef.set_description(pvarMonRecalage1, "fra", "Moniteur de Recalage1");
    predef.set_description(pvarMonRecalage1, "eng", "[eng] Moniteur de Recalage1");
    predef.set_description(pvarMonRecalage1, "ger", "[ger] Moniteur de Recalage1");
    predef.set_description(pvarMonRecalage1, "ita", "[ita] Moniteur de Recalage1");
    predef.set_description(pvarMonRecalage1, "spa", "[spa] Moniteur de Recalage1");
    predef.set_description(pvarMonRecalage1, "cat", "[cat] Moniteur de Recalage1");
  
    
    --Definition des groupes 400T
    --local train_number = util.get_train_serial_num();
    
    player400t.set_location_prefix("Location_");
    player400t.register_display("CAR-1.Location_30", 1, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-1.Location_31", 1, player400t.ROOM_UPSTAIRS_SIDE_2);
    player400t.register_display("CAR-1.Location_32", 1, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-1.Location_33", 1, player400t.ROOM_DOWNSTAIRS_SIDE_2); 
    
    player400t.register_display("CAR-2.Location_30", 2, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-2.Location_31", 2, player400t.ROOM_UPSTAIRS_SIDE_2); 
    player400t.register_display("CAR-2.Location_32", 2, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-2.Location_33", 2, player400t.ROOM_DOWNSTAIRS_SIDE_2); 
    
    player400t.register_display("CAR-3.Location_30", 3, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-3.Location_31", 3, player400t.ROOM_UPSTAIRS_SIDE_2);
    player400t.register_display("CAR-3.Location_32", 3, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-3.Location_33", 3, player400t.ROOM_DOWNSTAIRS_SIDE_2);   

    player400t.register_display("CAR-5.Location_30", 5, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-5.Location_31", 5, player400t.ROOM_UPSTAIRS_SIDE_2);
    player400t.register_display("CAR-5.Location_32", 5, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-5.Location_33", 5, player400t.ROOM_DOWNSTAIRS_SIDE_2); 
    
    player400t.register_display("CAR-4.Location_30", 4, player400t.ROOM_UPSTAIRS_SIDE_1);
    
    player400t.register_display("CAR-6.Location_30", 6, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-6.Location_31", 6, player400t.ROOM_UPSTAIRS_SIDE_2); 
    player400t.register_display("CAR-6.Location_32", 6, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-6.Location_33", 6, player400t.ROOM_DOWNSTAIRS_SIDE_2); 
    
    player400t.register_display("CAR-7.Location_30", 7, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-7.Location_31", 7, player400t.ROOM_UPSTAIRS_SIDE_2);
    player400t.register_display("CAR-7.Location_32", 7, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-7.Location_33", 7, player400t.ROOM_DOWNSTAIRS_SIDE_2);     

    player400t.register_display("CAR-8.Location_30", 8, player400t.ROOM_UPSTAIRS_SIDE_1);
    player400t.register_display("CAR-8.Location_31", 8, player400t.ROOM_UPSTAIRS_SIDE_2);
    player400t.register_display("CAR-8.Location_32", 8, player400t.ROOM_DOWNSTAIRS_SIDE_1);
    player400t.register_display("CAR-8.Location_33", 8, player400t.ROOM_DOWNSTAIRS_SIDE_2);         
    
    
    player400t.add_display_group("AIV-PREMIERE-CLASSE", "CAR-1.Location_30",  
                                                        "CAR-1.Location_31",
                                                        "CAR-1.Location_32");
                                                        
    player400t.add_display_group("AIV-SECONDE-CLASSE",  "CAR-4.Location_30",
                                                        "CAR-5.Location_30",
                                                        "CAR-5.Location_31",
                                                        "CAR-5.Location_32",
                                                        "CAR-5.Location_33",
                                                        "CAR-6.Location_30",
                                                        "CAR-6.Location_31",
                                                        "CAR-6.Location_32",
                                                        "CAR-6.Location_33",
                                                        "CAR-7.Location_30",
                                                        "CAR-7.Location_31",
                                                        "CAR-7.Location_32",
                                                        "CAR-7.Location_33",
                                                        "CAR-8.Location_30",
                                                        "CAR-8.Location_31",
                                                        "CAR-8.Location_32",
                                                        "CAR-8.Location_33");
                                                        
    
    evtmgr.add_mission_event(evtmgr.MISSION_NOT_INIT, Display150TEventNI, evtmgr.LANGUAGE_LIST_BY_CALL);
    evtmgr.add_mission_event(evtmgr.MISSION_NOT_INIT, Display400TEventNI, evtmgr.LANGUAGE_LIST_BY_CALL);
    evtmgr.add_mission_event(evtmgr.MISSION_INIT, Display400TEventMI, evtmgr.LANGUAGE_LIST_BY_CALL);
    evtmgr.add_mission_event(evtmgr.MISSION_INIT, PancartageNormal150T, evtmgr.LANGUAGE_LIST_BY_CALL);
    
    
    
    
    evtmgr.run();
    
    predef.run();
    
  end;
  
  util.trace( "predef.lua done", util.L5 );
  
end;	

function _Finalization()
  
  playermp3.close();
  evtmgr.stop();
  
  predef.stop();
  
  -- Déconnexion du dictionnaire
  dictionary.disconnect(dic);
  
  -- Destruction du dictionnaire
  dictionary.destroy(dic);
    
end;
timerCount = 0

function Display400TEventNI(pState, pLanguage, pVarArgsTable)
 
  
   
   
   timer1 = timer.create(onTimer, 2000)
   timer.start(timer1)
  
  
end

function Display400TEventMI(pState, pLanguage, pVarArgsTable)
 timerCount = 0
   timer.stop(timer1)
  
end

function onTimer()
util.trace("onTimer ", util.L5);
timerCount = timerCount + 1
if(timerCount > 1) then
  util.trace("Display400TEventNI function called ", util.L5);
  timer.stop(timer1)
   player400t.play(priority.PERMANENT_IDLE, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[evtmgr.MISSION_NOT_INIT], 900, tag.TRAIN_MESSAGE, pVarArgsTable)
   end

end
  
function FreeMsgFunction1LanguageByCall(pMsgRef, delay, pVarArgs)
  local lFreeText = "";  
  util.trace( "Playing Free text message, 150T", util.LW );
  
  if (predef.has_free_text_param(pMsgRef)) then
    lFreeText = predef.get_free_text_param(pMsgRef); 
  end;
  
  -- Display on all the 150t
  local page = player150t.create_page_from_template( "TemplateAED06");
  player150t.set_variable_value( page, "$FreeText", lFreeText); 
  
  local lPages = {}
  lPages[1] = page;  
  player150t.play( priority.PREDEFINED_LOW, player150t.GROUP_ALL_DISPLAYS, delay, player150t.INFINITE_DURATION, 1000, tag.INSTANT_MESSAGE, lPages); 
  
end;


function OnAttention(pMsgRef, pVarArgs) 
  util.trace(string.format("Display Attention Getter"), util.LW);  
  player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, mission_pack.WEB_ROOT.."fra/Attention.php", 20, tag.INSTANT_MESSAGE);   
end;


function OnFreeText(pMsgRef, pDuration,  pVarArgs)

  if (predef.has_free_text_param(pMsgRef)) then
    messageToDisplay = string.format("Information: %s", predef.get_free_text_param(pMsgRef))
    
   	local test_time = string.match(messageToDisplay, "time");
    local test_number = string.match(messageToDisplay, "number");
	local test_greet = string.match(messageToDisplay, "greet");
	local test_bye = string.match(messageToDisplay, "bye");
	local test_delay = string.match(messageToDisplay, "delay");
	local test_type = string.match(messageToDisplay, "type");
	local test_station = string.match(messageToDisplay, "station");
	local test_stations = string.match(messageToDisplay, "statslist");
    
    if (test_time == "time") then
        local time = string.sub(messageToDisplay, 19);
        util.trace(string.format("Enounce %s", time), util.L5);
		
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
		
        audioMsg = speech.time(time, language.FRA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.time(time, language.ENG);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.time(time, language.GER);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.time(time, language.SPA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.time(time, language.ITA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.time(time, language.CAT);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
        
    else if (test_number == "number") then
        local number = string.sub(messageToDisplay, 21);
        util.trace(string.format("Enounce %s", number), util.L5);
    
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
		
        audioMsg = speech.number(number, language.FRA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.digits(number, language.ENG);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
		-- CR 336632
		if (number == "0") then
			audioMsg = speech.digits(number, language.GER);
		else
			audioMsg = speech.number(number, language.GER);
		end;
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.number(number, language.SPA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.number(number, language.ITA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.number(number, language.CAT);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE);
		
	else if (test_greet == "greet") then
	
		local time = string.sub(messageToDisplay, 20);
		util.trace(string.format("Enounce greetings at %s", time), util.L5);
		
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predefIhm.SCOPE_NOT_IHM, tag.INSTANT_MESSAGE)
		
		w = speech.greeting(time, language.FRA, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.ENG, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.GER, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.SPA, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.ITA, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.CAT, speech.GREETING_BEGIN)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
	else if (test_bye == "bye") then
	
		local time = string.sub(messageToDisplay, 17);
		util.trace(string.format("Enounce bye at %s", time), util.L5);
		
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predefIhm.SCOPE_NOT_IHM, tag.INSTANT_MESSAGE)
		
		w = speech.greeting(time, language.FRA, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.ENG, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.GER, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.SPA, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.ITA, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.greeting(time, language.CAT, speech.GREETING_END)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);	
		
	else if (test_delay == "delay") then
	
		local delay = string.sub(messageToDisplay, 20);
		util.trace(string.format("Enounce delay: %s", delay), util.L5);
		
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predefIhm.SCOPE_NOT_IHM, tag.INSTANT_MESSAGE)
		
		w = speech.delay(delay, language.FRA)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.delay(delay, language.ENG)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.delay(delay, language.GER)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.delay(delay, language.SPA)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);
		
		w = speech.delay(delay, language.ITA)  
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);	

		w = speech.delay(delay, language.CAT) 
		playerwav.play(priority.TRAIN_EVENT_LOW,lScope,w,tag.INSTANT_MESSAGE);	

    else if (test_type == "type") then
        local type = string.sub(messageToDisplay, 19);
        util.trace(string.format("Enounce type %s", type), util.L5);
    
		local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
        
		audioMsg = speech.mission_type(type, language.FRA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.mission_type(type, language.ENG);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.mission_type(type, language.GER);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.mission_type(type, language.SPA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.mission_type(type, language.ITA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.mission_type(type, language.CAT);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 	
	
	else if (test_station == "station") then
        local station = string.sub(messageToDisplay, 22);
        util.trace(string.format("Enounce station %s", station), util.L5);

        local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
		
        audioMsg = speech.station(lmtdb.get_station_code(station), language.FRA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.station(lmtdb.get_station_code(station), language.ENG);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.station(lmtdb.get_station_code(station), language.GER);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.station(lmtdb.get_station_code(station), language.SPA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.station(lmtdb.get_station_code(station), language.ITA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.station(lmtdb.get_station_code(station), language.CAT);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 	
	
	else if (test_stations == "statslist") then
        local stations = dictionary.getvalue(dic, "Mission.StationList.IUICCodes")
        util.trace(string.format("Enounce stations list %s", stations), util.L5);

        local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
		
        audioMsg = speech.stations(stations, language.FRA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.stations(stations, language.ENG);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.stations(stations, language.GER);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.stations(stations, language.SPA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.stations(stations, language.ITA);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
    
        audioMsg = speech.stations(stations, language.CAT);
        playerwav.play(priority.PREDEFINED_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE); 
	
	end;
	end;
    end;
    end;
	end;	
	end;
    end;   
    end;
    
    messageToDisplay = util.url_encode(messageToDisplay)
    msgId = predef.get_msg_id(pMsgRef); 
    util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);
    
    
    
    player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, pDuration, tag.INSTANT_MESSAGE);   
  end;   
end;


function OnAutoDelay1(pMsgRef, pLanguage, DelayValue, pVarArgs)   

  local lUserData = {  ["ihmBroadcastScope"] =   predef.get_broadcast_scope(pMsgRef),
                        ["estDelay"] =            DelayValue}   
                        
  msgId = predef.get_msg_id(pMsgRef);
  
  stationId = dictionary.getvalue(dic, "Mission.EventStation.InternalID")
  if(stationId == nil)
  then
    stationLongName = "Station ID introuvable ";
    stationLongName = lmtdb.get_station_long_name(stationId, pLanguage) 
  else
    stationLongName = lmtdb.get_station_long_name(stationId, pLanguage) 
    util.trace("OnAutoDelay, stationLongName = "..stationLongName, util.LW);
  end;  
  
  messageToDisplay = "Le retard à l'arrivée de "..stationLongName.." est estimé à "..DelayValue.. " mins.";
  
  messageToDisplay_Eng = "The delay to the arrival to "..stationLongName.." is estimated to be "..DelayValue.. " mins.";
  
  local delayValidity = dictionary.getvalue(dic, "Mission.DelayValidity");
  util.trace("OnAutoDelay, DelayValidity = "..delayValidity.." , DelayValue = "..DelayValue, util.LW);
    
  
  if( delayValidity == 1 and DelayValue > 0) 
  then
      if ( pLanguage == language.FRA ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce FRA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData);           
      elseif( pLanguage == language.ENG ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce ENG %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay_Eng, 20, tag.DELAY_MESSAGE, lUserData);           

      elseif( pLanguage == language.GER ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce GER %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData);           

      elseif( pLanguage == language.ITA ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce ITA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData);           

      elseif( pLanguage == language.SPA ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce SPA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData);           
	  elseif( pLanguage == language.CAT ) 
      then
        util.trace(string.format("Visual and Audio Auto Delay Announce CAT %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData);           
      end;      
  end;   

end;

function OnAutoDelay(pMsgRef, pLanguage, DelayValue, pVarArgs)   

  local lUserData = {  ["ihmBroadcastScope"] =   predef.get_broadcast_scope(pMsgRef),
                        ["estDelay"] =            DelayValue}
                        
  msgId = predef.get_msg_id(pMsgRef);   
  messageToDisplay = msgId.."|"..DelayValue
  local delayValidity = dictionary.getvalue(dic, "Mission.DelayValidity");
  util.trace("OnAutoDelay, DelayValidity = "..delayValidity.." , DelayValue = "..DelayValue, util.LW);

  if( delayValidity == 1 and DelayValue > 0) 
  then
      if ( pLanguage == language.FRA ) 
      then
        messageToDisplay = string.format("Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce FRA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardFraWebPage);           
      elseif( pLanguage == language.ENG ) 
      then
        messageToDisplay = string.format("Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce ENG %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardEngWebPage);           

      elseif( pLanguage == language.GER ) 
      then
        messageToDisplay = string.format("Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce GER %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardGerWebPage);           

      elseif( pLanguage == language.ITA ) 
      then
        messageToDisplay = string.format("Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce ITA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardItaWebPage);           

      elseif( pLanguage == language.SPA ) 
      then
        messageToDisplay = string.format("Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce SPA %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardSpaWebPage);           
      
	  elseif( pLanguage == language.CAT ) 
      then
        messageToDisplay = string.format("(cat) Notre train est actuellement en retard de %d mn.", DelayValue)
        util.trace(string.format("Visual and Audio Auto Delay Announce CAT %s", messageToDisplay), util.L5);
        player400t.play(priority.EMERGENCY_ALARM, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.DELAY_MESSAGE, lUserData, OnSyncRetardCatWebPage);           
      end;      
  end;   
  
end;



function OnTaxiAnnounce(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
 local lUserData = { ["ihmBroadcastScope"] = predef.get_broadcast_scope(pMsgRef) }
  msgId = predef.get_msg_id(pMsgRef); 
  
  if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
  then
    util.trace("Audio Part Only", util.L5);
    g_VisualInhibFlag = true;
  else
    util.trace("Audio and Visual Part Only", util.L5);      
    g_VisualInhibFlag = false; 
  end; 
  
  if ( pLanguage == language.FRA ) 
  then
    util.trace(string.format("Announce %s", gFraPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gFraPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiFraWebPage);
    
  elseif( pLanguage == language.ENG ) 
  then
    util.trace(string.format("Announce %s", gEngPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gEngPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiEngWebPage);
    
  elseif( pLanguage == language.GER ) 
  then
    util.trace(string.format("Announce %s", gGerPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gGerPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiGerWebPage);
    
  elseif( pLanguage == language.ITA ) 
  then
    util.trace(string.format("Announce %s", gItaPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gItaPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiItaWebPage);
    
  elseif( pLanguage == language.SPA ) 
  then
    util.trace(string.format("Announce %s", gSpaPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gSpaPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiSpaWebPage);
	
  elseif( pLanguage == language.CAT ) 
  then
    util.trace(string.format("(cat) Announce %s", gCatPredefWebPages[msgId]), util.L5);
    player400t.play(priority.PREDEFINED_LOW, "AIV-PREMIERE-CLASSE", gCatPredefWebPages[msgId], 30, tag.INSTANT_MESSAGE, lUserData, OnSyncTaxiCatWebPage);
  end;  
end;


function OnDoorsLockedAnnounce(pMsgRef, pLanguage, pVarArgs)
 local lUserData = { ["ihmBroadcastScope"] = predef.get_broadcast_scope(pMsgRef) }
  msgId = predef.get_msg_id(pMsgRef); 
   messageToDisplay = msgId.."|"
   
  if ( pLanguage == language.FRA ) then
  
   -- messageToDisplay = string.format("Le depart de notre TGV est imminent. Attention à la fermeture des portes");
   
    --util.trace(string.format("Announce %s", gFraPredefWebPages[msgId]), util.L5);
   util.trace(string.format("Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedFraWebPage);
    
   elseif( pLanguage == language.ENG ) then
  
    --util.trace(string.format("Announce %s", gEngPredefWebPages[msgId]), util.L5);
   util.trace(string.format("Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedEngWebPage);
   
   elseif( pLanguage == language.GER ) then
  
    --util.trace(string.format("Announce %s", gGerPredefWebPages[msgId]), util.L5);
   util.trace(string.format("Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedGerWebPage);
    
    
    elseif( pLanguage == language.ITA ) then
  
    --util.trace(string.format("Announce %s", gItaPredefWebPages[msgId]), util.L5);
   util.trace(string.format("Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedItaWebPage);
    
    
    elseif( pLanguage == language.SPA ) then
  
    --util.trace(string.format("Announce %s", gSpaPredefWebPages[msgId]), util.L5);
   util.trace(string.format("Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedSpaWebPage);
    
    elseif( pLanguage == language.CAT ) then
  
    --util.trace(string.format("Announce %s", gCatPredefWebPages[msgId]), util.L5);
   util.trace(string.format("(cat) Display Doors locked Announce %s", messageToDisplay), util.L5);
    player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncDoorsLockedCatWebPage);
    
  end;
  
  Display150TEventDoorsLocked(pLanguage);
end;

function OnBarAnnounce(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
 local lUserData = { ["ihmBroadcastScope"] =    predef.get_broadcast_scope(pMsgRef),
                      ["voiture"] =             predef.get_carid_param(pMsgRef) } 
  
  local PrefixCarNumber = dictionary.getvalue(dic, "Train.PrefixCarNumber")
  if(PrefixCarNumber ~= nil)
  then
  
    CarID_IHM = lUserData["voiture"];
    Car_Len = string.len(CarID_IHM);
    
    -- Keep only the non-prefixed car number
    if (Car_Len > 1)
    then
      Org_CarID = string.sub(CarID_IHM, -1)
    else
      Org_CarID = CarID_IHM
    end
    
    -- Add prefix
    if (PrefixCarNumber ~= 0)
    then
      Prefixed_CarID = PrefixCarNumber..Org_CarID; 
    else
      Prefixed_CarID = Org_CarID;
    end
    
    util.trace("*** [BAR] Original Car: "..CarID_IHM..", Prefix: "..PrefixCarNumber..", Modified car: "..Prefixed_CarID.." ***", util.L5);
    
    -- Save car
    lUserData["voiture"] = Prefixed_CarID
  end;   
  
  msgId = predef.get_msg_id(pMsgRef);  
  if (predef.has_carid_param(pMsgRef)) 
  then
    messageToDisplay = msgId.."|"..lUserData["voiture"]   
    if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
    then
      util.trace("Audio Part Only", util.L5);
      g_VisualInhibFlag = true;
    else
      util.trace("Audio and Visual Part Only", util.L5);      
      g_VisualInhibFlag = false; 
    end; 
    

  
    if ( pLanguage == language.FRA ) then
      --messageToDisplay = string.format("Mesdames, Messieurs, pour vous restaurer, un bar est à votre disposition en voiture %d.", lUserData["voiture"]);   
      
      util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);             
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarFraWebPage);
      
    elseif ( pLanguage == language.ENG ) then
      --messageToDisplay = string.format("Food and beverages will shortly be available at the bar counter in coach %d.", lUserData["voiture"]);   
      
    util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);             
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarEngWebPage);
      
    elseif ( pLanguage == language.GER ) then
      --messageToDisplay = string.format("Food and beverages will shortly be available at the bar counter in coach %d.", lUserData["voiture"]);   
      
     util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);        
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarGerWebPage);
      
    elseif ( pLanguage == language.ITA ) then
      --messageToDisplay = string.format("Food and beverages will shortly be available at the bar counter in coach %d.", lUserData["voiture"]);   
      
     util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);            
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarItaWebPage);
      
    elseif ( pLanguage == language.SPA ) then
      --messageToDisplay = string.format("Food and beverages will shortly be available at the bar counter in coach %d.", lUserData["voiture"]);   
      
     util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);            
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarSpaWebPage);
	  
	elseif ( pLanguage == language.CAT ) then
      --messageToDisplay = string.format("Food and beverages will shortly be available at the bar counter in coach %d.", lUserData["voiture"]);   
      
     util.trace(string.format("(cat) Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);            
      player400t.play(priority.PREDEFINED_LOW, "AIV-SECONDE-CLASSE", gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncBarCatWebPage);
	  
    end; 
  end;
end;

function OnDoctorAnnounce(pMsgRef, pLanguage, pVarArgs)    
  msgId = predef.get_msg_id(pMsgRef);     
  if (predef.has_carid_param(pMsgRef)) then
    carid = predef.get_carid_param(pMsgRef); 
      
    messageToDisplay = msgId.."|"..carid
    if ( pLanguage == language.FRA ) then
     -- messageToDisplay = string.format("Un médecin est demandé en voiture %d.", carid); 
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
    elseif ( pLanguage == language.ENG ) then
      --messageToDisplay = string.format("A doctor is needed in car %d.", carid); 
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
     elseif ( pLanguage == language.GER ) then
      --messageToDisplay = string.format("A doctor is needed in car %d.", carid); 
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
     elseif ( pLanguage == language.ITA ) then
      --messageToDisplay = string.format("A doctor is needed in car %d.", carid); 
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
     elseif ( pLanguage == language.SPA ) then
      --messageToDisplay = string.format("A doctor is needed in car %d.", carid); 
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
    elseif ( pLanguage == language.CAT ) then
      --messageToDisplay = string.format("A doctor is needed in car %d.", carid); 
      util.trace(string.format("(cat) Display Announce %s", messageToDisplay), util.L5);             
      player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE);
      
      
    end;
  end;   
end;

function OnDelayAnnounce(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
  msgId = predef.get_msg_id(pMsgRef);
  
  if (predef.has_station_param(pMsgRef)) 
  then
    stationId = predef.get_station_param(pMsgRef);
    stationCode = lmtdb.get_station_code(stationId);
    stationLongName = lmtdb.get_station_long_name(stationId, pLanguage)  
    if (predef.has_delay_param(pMsgRef)) 
    then
        delay = predef.get_delay_param(pMsgRef); 
         messageToDisplay = msgId.."|"..stationLongName.."|"..stationCode.."|"..delay
         
        if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
        then
          util.trace("Audio Part Only", util.L5);
          g_VisualInhibFlag = true;
        else
          util.trace("Audio and Visual Part Only", util.L5);      
          g_VisualInhibFlag = false; 
        end; 
        if ( pLanguage == language.FRA ) then
          util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
          
         elseif( pLanguage == language.ENG ) then
          util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
          
         elseif( pLanguage == language.GER ) then
          util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
          
          elseif( pLanguage == language.ITA ) then
          util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
          
          elseif( pLanguage == language.SPA ) then
          util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
		  
		  elseif( pLanguage == language.CAT ) then
          util.trace(string.format("(cat) Display Announce %s", messageToDisplay), util.L5);                               
          player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE); 
        end;
    end;    
  end;
end;


function OnEstimatedDelayAnnounce(pMsgRef, pLanguage, pArgs)  
 g_VisualInhibFlag = false;   
 local lUserData = {  ["ihmBroadcastScope"] =   predef.get_broadcast_scope(pMsgRef),
                      ["estDelay"] =            predef.get_delay_param(pMsgRef) }
  msgId = predef.get_msg_id(pMsgRef);
  A103_msgID = msgId;
  A103_pLanguage = pLanguage; 
  if (predef.has_delay_param(pMsgRef)) 
  then
    messageToDisplay = msgId.."|"..lUserData["estDelay"]
    
    if (lUserData["estDelay"] > 0) 
    then
      if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
      then
        util.trace("Audio Part Only", util.L5);
        g_VisualInhibFlag = true;
      else
        util.trace("Audio and Visual Part Only", util.L5);      
        g_VisualInhibFlag = false; 
      end; 
      if ( pLanguage == language.FRA ) 
      then
        util.trace(string.format("Display Announce FRA", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelayFraWebPage); 

      elseif ( pLanguage == language.ENG ) 
      then
        util.trace(string.format("Display Announce ENG", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelayEngWebPage); 

      elseif ( pLanguage == language.GER ) 
      then
        util.trace(string.format("Display Announce GER", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelayGerWebPage); 

      elseif ( pLanguage == language.ITA ) 
      then
        util.trace(string.format("Display Announce ITA", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelayItaWebPage); 

      elseif ( pLanguage == language.SPA) 
      then
        util.trace(string.format("Display Announce SPA", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelaySpaWebPage); 
		
	  elseif ( pLanguage == language.CAT) 
      then
        util.trace(string.format("Display Announce CAT", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncEstimatedDelayCatWebPage); 
      end; 
    end;
  end; 
end;

function OnTestSonoDiff(pMsgRef, pLanguage, pVarArgs)

  local lUserData = { ["ihmBroadcastScope"] = predef.get_broadcast_scope(pMsgRef) }
  lUserData["voiture"]  = predef.get_carid_param(pMsgRef) 
  
  if (string.len(lUserData["voiture"]) == 1) then
    lUserData["voiture"] = "0"..lUserData["voiture"];
  end;
  parseTag = tonumber(string.sub(lUserData["voiture"], 1, 1));
  if (parseTag == 0) then
    annonceTag = tag.INSTANT_MESSAGE
  elseif (parseTag == 1) then
   annonceTag = tag.MISSION_MESSAGE
  elseif (parseTag == 2) then
   annonceTag = tag.TRAIN_MESSAGE
  elseif (parseTag == 3) then
   annonceTag = tag.DELAY_MESSAGE
  end;

  util.trace(string.format("Test Sono Diffusion, Mode %s", lUserData["voiture"]), util.L5);
  local vars="SIV-ANNONCE-PRED-VOITURE-PARAMS|SIE-SONO-DIFFUSION-REQ|AS.SelectChannel|AS.SelectedChannelNum|SIV-ANNONCE-PRED-UM|SIV-PRED-INHIB-ETAT|SIV-AUTO-INHIB-ETAT|SIV-RETARD-INHIB-ETAT|SIE-COUPLAGE|SIV-COUPLAGE-TYPE";
  player400t.play(priority.PREDEFINED_HIGH, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 15, annonceTag, lUserData, OnSyncTestSonoDiffusion); 
          
end;

function OnVarMonRetard(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Retard"), util.L5);   
  local vars="SIV-RETARD-ACTIF-ETAT|SIV-RETARD-AFFICHE|SIV-RETARD-MESURE-ARRONDI|SIV-RETARD-MODE|SIV-RETARD-REEL|SIV-RETARD-MSG-ID|SIV-RETARD-TYPE-DIFFUSION";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonLum(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Master"), util.L5);   
  local vars="SIE-ECLAIRAGE-REDUIT-ELEM|SIE-HT|SRS-INTENSITE-ETAT|SIV-AIV-INTENSITE-ETAT|Train.EnergySaving.State";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;
  
function OnVarMonStatut(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="SIE-MODE-VV|SIE-AUTORISATION-DIFFUSION|SIV-MODE-OPERATIONNEL-ETAT|SRS-ACT-ETAT|SIV-SRS-PRESENT|System.Master.PIS.CarNum|System.Master.UMC.CarNum|System.VMC.Ready|System.PIS.Ready|SIV-GPS-ETAT";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;


function OnVarMonSante1(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="IHM-HB-R2|IHM-HB-R3|IHM-HB-R4|IHM-HB-R6|IHM-HB-R8|PIS-HB-R4|PIS-HB-R6";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonSante2(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="System.SIE.HB_R4|System.SIE.HB_R6|System.SIE.LinkStatus.R4|System.SIE.LinkStatus.R6|VMC-HB-R4|VMC-HB-R6";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;


function OnVarMonDateHeure(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="DateTime.Local|DateTime.UTC|SIE-DATE-HEURE|Train.GPS.DateTime|LMT.Input.LocalDateTime|SIV-DATE-HEURE";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonVersion1(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="IHM-MAINT-VERSION-R2|IHM-MAINT-VERSION-R3|IHM-MAINT-VERSION-R4|IHM-MAINT-VERSION-R6|IHM-MAINT-VERSION-R8";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonVersion2(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="SIV-MAINT-APP-VERSION-PIS|SIV-MAINT-APP-VERSION-PIS-AED|SIV-MAINT-APP-VERSION-PIS-AIV|SIV-MAINT-APP-VERSION-SRS|SIV-MAINT-APP-VERSION-SRS-ARS|SIV-MAINT-APP-VERSION-VMC|SIV-MAINT-BDD-VERSION-LMT|SIV-MAINT-BDD-VERSION-SIV-BASE|SIV-MAINT-BDD-VERSION-SIV-MISSION";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonGPS(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="LMT.Input.GPSLockStatus|LMT.Input.GPSCommunicationStatus|SIV-GPS-ETAT|Train.GPS.Valid|Train.GPS.SpeedInKmh|Train.GPS.Longitude|Train.GPS.Latitude";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonRecalage1(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut Recalage"), util.L5);   
  local vars ="Mission.TotalDistance|Mission.TotalRemainingDistance|Mission.SegmentDistance|Mission.SegmentRemainingDistance|LMT.Output.NextRuntimePoint|Mission.Recalage.Etat|LMT.Output.TrainPositionEstimationPrecision|LMT.Input.GpsCurrentSource|LMT.Output.TrainPositionSource";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonLMT(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="LMT.State|Mission.State|Mission.PreviousStation.ShortName[fra]|Mission.EventStation.ShortName[fra]|Mission.NextStation.ShortName[fra]|LMT.Output.NextRuntimePoint|LMT.Input.OdometerStatus|Mission.SegmentDistanceTraveled|Mission.SegmentDistance|Mission.SegmentRemainingDistance";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;

function OnVarMonCouplage(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Display Variable Monitor de Statut"), util.L5);   
  local vars ="SIV-COUPLAGE-TYPE|SIE-COUPLAGE|SIV-MISSION-ACT-DISTANT-ETAT|SIV-MISSION-ACT-DISTANT-PREFIXE-LOCAL";  
  player400t.play(priority.TRAIN_EVENT_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages["VariableMonitor"]..vars, 1000000, tag.INSTANT_MESSAGE); 
          
end;
function OnAudioOnly(pMsgRef, pLanguage, pVarArgs)

  util.trace(string.format("Playing Audio Message only, priority TRAIN_EVENT_LOW"), util.L5);   
  audioMsg = playerwav.concat( speech.sentence("F_MDSMRS.WAV", language.FRA ), speech.digits(987654321, language.FRA ));
  
  -- diffusion du message concaténé précédement avec la priorité 0 pour tout l'élément
  local lScope = predef.adjust_broadcast_scope(playerwav.AUDIO_GROUP_TRAIN, predef.get_broadcast_scope(pMsgRef), tag.INSTANT_MESSAGE)
  playerwav.play(priority.TRAIN_EVENT_LOW, lScope, audioMsg, tag.INSTANT_MESSAGE);

  
end;



function OnPlayMP3(pMsgRef, pLanguage, pVarArgs)
  playermp3.open(base_pack.SOUND_DIR.."Music.mp3");
  playermp3.play(playerwav.AUDIO_GROUP_TRAIN);
  util.trace( "Opening and Playing MP3", util.LW )
end;

function OnPauseMP3(pMsgRef, pLanguage, pVarArgs)
  
    playermp3.pause();
    util.trace( "pausing MP3", util.LW )
end;

function OnStopMP3(pMsgRef, pLanguage, pVarArgs)
  
    playermp3.stop();
    playermp3.close();
    util.trace( "Stopping and Closing MP3", util.LW )
end;


function OnPlayPinkNoiseMP3(pMsgRef, pLanguage, pVarArgs)
  playermp3.open(base_pack.SOUND_DIR.."PinkNoise.mp3");
  playermp3.play(playerwav.AUDIO_GROUP_TRAIN);
  util.trace( "Opening and Playing Pink Noise MP3", util.LW )
end;


function OnSetGong(pMsgRef, pLanguage, pVarArgs)
  
  if (predef.has_delay_param(pMsgRef) and predef.has_carid_param(pMsgRef) ) then
  
        local trigTime = predef.get_delay_param(pMsgRef) + 1;
        local gongType = predef.get_carid_param(pMsgRef);
        
        util.trace( "Setting the Gong: Time to"..trigTime..", Type to:"..gongType, util.LW )
        playerwav.set_gong_trig_time(trigTime);
        playerwav.set_gong(base_pack.SOUND_DIR.."misc/Gong"..gongType..".wav");
      
    
    end;
end;


function OnDelayWeatherAnnounce(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
 local lUserData = {  ["ihmBroadcastScope"] =   predef.get_broadcast_scope(pMsgRef),
                      ["weatherDelay"] =        predef.get_delay_param(pMsgRef) }
  msgId = predef.get_msg_id(pMsgRef);
  
  if (predef.has_delay_param(pMsgRef)) 
  then
    messageToDisplay = msgId.."|"..lUserData["weatherDelay"]  
    if (lUserData["weatherDelay"] > 0) 
    then
      if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
      then
        util.trace("Audio Part Only", util.L5);
        g_VisualInhibFlag = true;
      else
        util.trace("Audio and Visual Part Only", util.L5);      
        g_VisualInhibFlag = false; 
      end; 
        
      if ( pLanguage == language.FRA ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelayFraWebPage); 

      elseif ( pLanguage == language.ENG ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelayEngWebPage); 
        
      elseif ( pLanguage == language.GER ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelayGerWebPage); 

      elseif ( pLanguage == language.SPA ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelaySpaWebPage); 

      elseif ( pLanguage == language.ITA ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelayItaWebPage); 
		
	  elseif ( pLanguage == language.CAT ) 
      then
        util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
        player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData, OnSyncWeatherDelayCatWebPage); 
      end;
    end;    
  end;
end;


function OnUFRAnnounce(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
 local lUserData = {  ["ihmBroadcastScope"] = predef.get_broadcast_scope(pMsgRef),
                      ["stationCode"] = "" }
  msgId = predef.get_msg_id(pMsgRef);
  
  if (predef.has_station_param(pMsgRef)) 
  then
    stationId = predef.get_station_param(pMsgRef);
    lUserData["stationCode"] = lmtdb.get_station_code(stationId);
    stationLongName = lmtdb.get_station_long_name(stationId, pLanguage) 
    messageToDisplay = msgId.."|"..stationLongName 
    if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
    then
      util.trace("Audio Part Only", util.L5);
      g_VisualInhibFlag = true;
    else
      util.trace("Audio and Visual Part Only", util.L5);      
      g_VisualInhibFlag = false; 
    end; 
    if ( pLanguage == language.FRA ) 
    then
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5);                               
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFRFraWebPage);
      
    elseif ( pLanguage == language.ENG ) 
    then
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5); 
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gEngPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFREngWebPage);
      
    elseif ( pLanguage == language.SPA ) 
    then
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5); 
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gSpaPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFRSpaWebPage);
      
    elseif ( pLanguage == language.ITA ) 
    then
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5); 
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gItaPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFRItaWebPage);
  
    elseif ( pLanguage == language.GER ) 
    then
      util.trace(string.format("Display Announce %s", messageToDisplay), util.L5); 
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gGerPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFRGerWebPage);        

    elseif ( pLanguage == language.CAT ) 
    then
      util.trace(string.format("(cat) Display Announce %s", messageToDisplay), util.L5); 
      player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gCatPredefWebPages[msgId]..messageToDisplay, 30, tag.INSTANT_MESSAGE, lUserData, OnSyncUFRCatWebPage);        
    end;  
  end;
end;

function OnDoorBlocked(pMsgRef, pLanguage, pVarArgs)
 g_VisualInhibFlag = false;    
 util.trace("OnDoorBlocked function is called", util.L5);  
 
 local lUserData = { ["ihmBroadcastScope"] =    predef.get_broadcast_scope(pMsgRef),
                      ["voiture"] =             predef.get_carid_param(pMsgRef), 
                      ["estDelay"] =            predef.get_delay_param(pMsgRef),
                      ["stationCode"] =         ""}
  msgId = predef.get_msg_id(pMsgRef); 
  
  if (predef.has_carid_param(pMsgRef) and predef.has_station_param(pMsgRef)) 
  then
    stationId = predef.get_station_param(pMsgRef);
    lUserData["stationCode"] = lmtdb.get_station_code(stationId);
    stationLongName = lmtdb.get_station_long_name(stationId, pLanguage)  
    
    messageToDisplay = "Une porte bloquée en voiture "..lUserData["voiture"].." en gare de "..stationLongName.." cause un retard de "..lUserData["estDelay"].." mins." 

    if ( dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == 2 or dictionary.getvalue(dic, "SIV-AUTO-INHIB-ETAT") == "2")
    then
      util.trace("Audio Part Only", util.L5);
      g_VisualInhibFlag = true;
    else
      util.trace("Audio and Visual Part Only", util.L5);      
      g_VisualInhibFlag = false; 
    end;
    
    util.trace(string.format("Display Announce %s - %s", messageToDisplay, pLanguage), util.L5);             
    player400t.play(priority.PREDEFINED_LOW, player400t.GROUP_ALL_DISPLAYS, gFraPredefWebPages[msgId]..messageToDisplay, 20, tag.INSTANT_MESSAGE, lUserData); 
    
  end;
end;

function Display150TEventNI(pState, pLanguage, pArgs)

  util.trace("Display150TEventNI function called", util.L5);
  
  page_fra = player150t.create_page_from_template( "TemplateAED03");
  
  local s = "Train hors service"
  local s1 = "Ne pas monter"

  player150t.print_text( page_fra, 1, s);
  player150t.print_cr(page_fra, 1);
  player150t.print_text( page_fra, 1, s1);
  
  -- Send page to all displays
  --player150t.stop(player150t.GROUP_ALL_DISPLAYS, tag.MISSION_MESSAGE);
  player150t.play(OVERRIDE_ALL, player150t.GROUP_ALL_DISPLAYS, player150t.INFINITE_DURATION, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page_fra);

end;


function Display150TEventDoorsLocked(pLanguage)

  util.trace("Display150TEventDoorsLocked function called", util.L5);
  
  local mission_trainnumber = dictionary.getvalue(dic, "Mission.TrainNumber");
  
  -- Build French Page
  local page = player150t.create_page_from_template( "TemplateAED02");
  player150t.set_variable_value( page, "$MissionNumber", " " ..mission_trainnumber ); 
 
  if ( pLanguage == language.FRA ) then
 
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[fra]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[fra]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "Train au départ"
      local s1 = "Ne pas monter"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
  
  elseif ( pLanguage == language.ENG ) then
  
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[eng]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[eng]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "Departure of the train"
      local s1 = "Do not get in"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
      
  elseif ( pLanguage == language.GER ) then
  
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[ger]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[ger]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "Zug bei der Abfahrt"
      local s1 = "Steigen Sie nicht an Bord"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
      
      
   elseif ( pLanguage == language.ITA ) then
  
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[ita]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[ita]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "Partenzo del treno"
      local s1 = "Non montare"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
     
   elseif ( pLanguage == language.SPA ) then
  
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[spa]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[spa]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "Salida de Tren"
      local s1 = "No monte"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
	  
	elseif ( pLanguage == language.CAT ) then
  
      local mission_first_station = dictionary.getvalue(dic, "Mission.FirstStation.ShortName[cat]");
      local mission_last_station = dictionary.getvalue(dic, "Mission.LastStation.ShortName[cat]");
    
      player150t.print_text( page, 1, mission_first_station);
      player150t.print_text( page, 2, mission_last_station); 
      
      local s = "(cat) Salida de Tren"
      local s1 = "No monte"

      player150t.print_text( page, 3, s);
      player150t.print_cr(page, 3);
      player150t.print_text( page, 3, s1);
    
      -- Send page to all displays
      player150t.play(priority.EMERGENCY_ALARM, player150t.GROUP_ALL_DISPLAYS, 30, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page);
     
      
  end;

end;


function PancartageNormal150T(pState, pLanguage, pArgs)

  util.trace("PancartageNormal150T function called", util.L5);
 
       page_fra = player150t.create_page_from_template( "TemplateAED01");
     
      if( pArgs["Mission.Type"] == "IDTGV" ) then
           player150t.print_bmp(page_fra, 0 , 5)
      else
           player150t.print_bmp(page_fra, 0 , 3)
      end
     
      player150t.print_text(page_fra, 0, " "..pArgs["Mission.TrainNumber"]);
      player150t.print_text( page_fra, 1, (pArgs["Mission.FirstStation.ShortName[fra]"]));
      player150t.print_text( page_fra, 2, (pArgs["Mission.LastStation.ShortName[fra]"])); 
      local s_fra = string.gsub(pArgs["Mission.InterStationList.StationNames[fra]"], ",", "  ")
  
      player150t.print_text( page_fra, 3, s_fra);
      
    -- Send page to all displays
    player150t.stop(player150t.GROUP_ALL_DISPLAYS, tag.MISSION_MESSAGE);
    player150t.play(priority.MISSION_EVENT, player150t.GROUP_ALL_DISPLAYS, player150t.INFINITE_DURATION, player150t.INFINITE_DURATION, tag.MISSION_MESSAGE, page_fra);

end;