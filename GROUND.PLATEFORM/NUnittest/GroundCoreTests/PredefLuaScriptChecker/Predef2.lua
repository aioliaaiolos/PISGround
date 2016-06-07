function _Initialization()  
 
  gBarCar = 4
  gEstimatedDelay = 0
  gWeatherDelay = 0
  gStationCode = 0
  
  --Création d'un client pour le service de dictionnaire actif dans l'élément
  dic = dictionary.create();
  
  -- Connexion au dictionnaire des variables du système SIV
  if (dictionary.connect(dic, dictionary.TRAIN_LOCAL, nil)) then
  
    predef.init(dic);
	 
	util.trace( "predef.init done", util.LW );     
	
	pdefBar = predef.create_instant_msg("1", OnBarAnnounce);
	predef.set_class(pdefBar, "INFO");
	predef.set_description(pdefBar, "fra", "Un Bar en voiture...");
	predef.set_description(pdefBar, "eng", "Un Bar en voiture...");
	predef.set_description(pdefBar, "ger", "Un Bar en voiture...");
	predef.add_input_param(pdefBar, predef.CARID_PARAM);
 
	pdefAllParam = predef.create_instant_msg("2", OnImAllParam);
	predef.set_class(pdefAllParam , "INFO");
	predef.set_description(pdefAllParam , "fra", "IM all param");
	predef.set_description(pdefAllParam , "eng", "IM all param");
	predef.set_description(pdefAllParam , "ger", "IM all param");
	predef.add_input_param(pdefAllParam , predef.DELAY_PARAM);
	predef.add_input_param(pdefAllParam , predef.STATION_PARAM);
	predef.add_input_param(pdefAllParam , predef.CARID_PARAM);
	
	freeMsg = predef.create_free_msg("T001", OnFreeText, OnAttention);
	predef.set_class(freeMsg, "INFO");
	predef.set_description(freeMsg, "fra", "Message texte libre en Francais");
	predef.set_description(freeMsg, "eng", "Message texte libre en Francais");
	predef.set_description(freeMsg, "ger", "Message texte libre en Francais");
  
  	autoDelay = predef.create_auto_delay_msg("D001", OnAutoDelay);
	predef.set_class(autoDelay, "INFO");
	predef.set_description(autoDelay, "fra", "Message de Retard automatique");
	predef.set_description(autoDelay, "eng", "Message de Retard automatique");
	predef.set_description(autoDelay, "ger", "Message de Retard automatique");
	
	predef.run();
	
  end;
  
  util.trace( "predef.lua done", util.LW );
end;	

function _Finalization()
  
  predef.stop();
  
  -- Déconnexion du dictionnaire
  dictionary.disconnect(dic);
  
  -- Destruction du dictionnaire
  dictionary.destroy(dic);
	
end;

function OnBarAnnounce(pMsgRef, pLanguage, pVarArgs)
  msgId = predef.get_msg_id(pMsgRef);   
  
  if (predef.has_carid_param(pMsgRef)) then
	gBarCar = predef.get_carid_param(pMsgRef); 
	
	if ( pLanguage == language.FRA ) then
	  
	  util.trace(string.format("!!Mesdames, Messieurs, bar en voiture %d.", gBarCar), util.LE);
	  
	end;
  end;  
end;

function OnImAllParam(pMsgRef, pLanguage, pVarArgs)
  local msgId = predef.get_msg_id(pMsgRef);
  
  if (predef.has_delay_param(pMsgRef)) then
	
	local delay = predef.get_delay_param(pMsgRef);
	
	  if (predef.has_carid_param(pMsgRef)) then

		  local carId = predef.get_carid_param(pMsgRef);
		
		  if (predef.has_station_param(pMsgRef)) then
			
			local stationId = predef.get_station_param(pMsgRef);
			
			local stationCode = lmtdb.get_station_code(stationId);
			
			local stationLongName = lmtdb.get_station_long_name(stationId, pLanguage);
			
			if (pLanguage == language.FRA ) then
				
				local message1ToDisplay = string.format("Delay=%d,Car=%d,StationId=%d,Code=%s,Name=%s", delay, carId, stationId, stationCode, stationLongName);				
				util.trace(message1ToDisplay, util.LE);			  
			end;
		  end;
		end;
	  end;      
end;  


function OnAttention(pMsgRef, pVarArgs) 
  util.trace(string.format("Display Attention Getter"), util.LE);
  return 10000;
end;

function OnFreeText(pMsgRef, pDuration,  pVarArgs)

  if (predef.has_free_text_param(pMsgRef)) then
	messageToDisplay = string.format("Information: %s", predef.get_free_text_param(pMsgRef))
	msgId = predef.get_msg_id(pMsgRef); 
	util.trace(string.format("Display Announce %s", messageToDisplay), util.LE);
  end;   
end;

function OnAutoDelay(pMsgRef, pLanguage, pDelayValue, pVisualRendering, pAudioRendering, pVarArgs)   
  if (pLanguage == language.FRA ) then
	  local msgId = predef.get_msg_id(pMsgRef);   
	  local messageToDisplay = string.format("Auto delay msg: ID=%s, Delay=%s, V=%s, A=%s", msgId, tostring(pDelayValue), tostring(pVisualRendering), tostring(pAudioRendering))
	  util.trace(messageToDisplay, util.LE);
	  
	  local audioMsg = playerwav.concat(
		speech.sentence("RETARD.WAV", language.FRA )
		);
	  
	  playerwav.play(priority.PREDEFINED_HIGH, playerwav.AUDIO_GROUP_TRAIN, audioMsg, tag.DELAY_MESSAGE);
  end;	  
end;

