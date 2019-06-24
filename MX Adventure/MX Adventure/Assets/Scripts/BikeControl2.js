    //var force : Vector3;
    var car : Transform;
    var power : int;
    var MaxSpeed : int = 150;
    var currentSpeed : float;
    //private var trigfunction : Vector3;
    var ccamera : Camera;
    //private var max : Vector3;
    var fwheel : Transform;
    var bwheel : Transform;
    var fWheelCollider : WheelCollider;
    var bWheelCollider : WheelCollider;
    var gearRatio : int[];
    
    private var distance : float;
    private var startTime: float = 0;
    private var point: Vector3;
    private var object: GameObject;
    private var duration: float;
    private var beginningBikeSoundPitch : float;
    //var raycast : Transform;
    //var isgrounded : boolean;
    var CenterOfMass : Vector3;
    private var trickStartPosition : Vector3;
    private var crashed : boolean = false;
    private var buttonPressed : boolean = false;
    
    public var customGuiStyle : GUIStyle;
    
    //var flipped : boolean;
        
    function Start () 
    {
    	//max.Set(8,8,0);
    	GetComponent.<Rigidbody>().centerOfMass=CenterOfMass;
    	crashed = false;
    	
    	ccamera.transparencySortMode = TransparencySortMode.Perspective;
    	beginningBikeSoundPitch = GetComponent.<AudioSource>().pitch;
    	
    	ApplyBrake();
    } 
    
    function Update()
	{ 
		EngineSound();
	
		if (Input.GetMouseButtonDown(0) || buttonPressed)
		{ 
			var hit: RaycastHit; 
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			buttonPressed = true;
			
			if (Physics.Raycast(ray, hit))
			{ 
				startTime = Time.time; point = hit.point; 
				object = hit.transform.gameObject; 
				var name : String;
				name = object.name;
				
				if(name == "greenGrip")
					ApplyGas();
				else if(name == "brakeRotor")
					ApplyBrake();
			} 
		} 
		else if (Input.GetMouseButtonUp(0) || !buttonPressed)
		{ 
			buttonPressed = false;
			duration = Time.time - startTime; 
			
			if(currentSpeed > 0)
				ApplyGas(-20);
		} 
	}
       
    function FixedUpdate () 
    {			
		//rotate wheel image
		fwheel.Rotate(0,0,fWheelCollider.rpm/60*360*Time.deltaTime * -1);
		bwheel.Rotate(0,0,bWheelCollider.rpm/60*360*Time.deltaTime * -1);
		
		currentSpeed = 2*22/7*bWheelCollider.radius*bWheelCollider.rpm*60/1000;
		currentSpeed = Mathf.Round(currentSpeed);
	}
	
	function OnTriggerEnter (col : Collider)
	{
		if(col.name == "MeshPiece")
		{
			if(!crashed)
				gamestate.Instance.SetLives(gamestate.Instance.getLives() - 1);
			
			if(gamestate.Instance.getLives() <= 0)
			{
				Application.LoadLevel("gameover");
			}
			else
			{
				crashed = true;
			}
		}
		else if(col.name == "Coin")
		{
			if(!crashed)
				gamestate.Instance.SetScore(gamestate.Instance.GetScore() + 1);
		}
	}
	
	function OnGUI()
	{
		if(crashed)
		{
			GUI.depth = 10;

			GUI.Box(Rect((Screen.width - 150)/2, (Screen.height-75) / 2, 150, 75),"", GUIStyle(GUI.skin.box));

//			var logoX : int = (Screen.width - 300 ) / 2;
//			var logoY : int = (Screen.height - 450) / 2;

//			customGuiStyle.font = (Font)Resources.Load("Fonts/advlit");
			customGuiStyle = GUIStyle(GUI.skin.box);
			customGuiStyle.fontSize = 30;
			customGuiStyle.normal.textColor = Color.white;
			customGuiStyle.alignment = TextAnchor.MiddleCenter;
			customGuiStyle.normal.background = null;
			
			GUI.skin = null;

			if(GUI.Button(Rect((Screen.width-150)/2, (Screen.height-75) / 2, 150, 75),"Try Again", customGuiStyle))
			{
				Application.LoadLevel(Application.loadedLevel);
			}

			GUI.depth = 0;
		}
	}
	
    function ApplyGas()
    {
    	ApplyGas(power);
    }
    
    function ApplyGas(torque : int)
    {
    	if(IsGrounded())
    	{
	    	if(currentSpeed < MaxSpeed && !crashed)
	    	{
		    	bWheelCollider.brakeTorque = 0;
		    	
		    	if(bWheelCollider.motorTorque > 0 || torque > 0)
		    		bWheelCollider.motorTorque += torque;
	    	}
	    	else
	    		bWheelCollider.motorTorque = 0;
	    		
    		if(trickStartPosition != new Vector3(0,0,0))
    			checkForTrick(car.transform.position);
	    }
	    else//rotate since they are in the air
	    {
	    	if(trickStartPosition == new Vector3(0,0,0))
	    		trickStartPosition = car.transform.position;
	    		
	    	//bwheel.Rotate(0,0,bWheelCollider.rpm/60*360*Time.deltaTime * -1);
	    	car.transform.Rotate(0,0, 20 * Time.deltaTime);
	    }
    }
    
    function ApplyBrake()
    {
    	if(IsGrounded())
    	{
	    	if(!crashed)
	    	{
		    	bWheelCollider.brakeTorque += 30;
		       	bWheelCollider.motorTorque = 0;
	       	}
       	}
	    else//rotate since they are in the air
	    {
	    	car.transform.Rotate(0,0, 20 * Time.deltaTime * -1);
	    }
    }
    
    function IsGrounded(): boolean {
    	var grounded : boolean;
    	
    	//check if back wheel is on the ground
    	grounded = Physics.Raycast(bWheelCollider.transform.position, -Vector3.up, (bWheelCollider.radius + bWheelCollider.suspensionDistance + 0.1)*6);
    	
    	if(grounded)//check if front wheel is on the ground
    		grounded = Physics.Raycast(fWheelCollider.transform.position, -Vector3.up, (fWheelCollider.radius + fWheelCollider.suspensionDistance + 0.1)*6);
    		
	   return grounded;
	 }
       
    function LateUpdate () {
    	//After we move, adjust the camera to follow the player
        ccamera.transform.position = new Vector3(transform.position.x, transform.position.y, ccamera.transform.position.z);
    }
    
    function EngineSound()
    {
		for (var i = 0; i < gearRatio.length; i++){
			if(gearRatio[i]> currentSpeed){
				break;
			}
		}
		
		var gearMinValue : float = 0.00;
		var gearMaxValue : float = 0.00;
		
		if (i == 0){
			gearMinValue = 0;
		}
		else {
			gearMinValue = gearRatio[i-1];
		}
		
		if(i > gearRatio.Length - 1)
			i = gearRatio.Length - 1;
			
		gearMaxValue = gearRatio[i];
		var enginePitch : float = ((currentSpeed - gearMinValue)/(gearMaxValue - gearMinValue))+1;
		
		if(enginePitch >= beginningBikeSoundPitch)//keep it from going lower than starting value
			GetComponent.<AudioSource>().pitch = enginePitch;
	}
	
	//check the position of the vehicle to see if he did a trick while in the air
	function checkForTrick(trickEndPosition : Vector3)
	{
		var xDifference : float = 0;
		
		xDifference = trickEndPosition.x - trickStartPosition.x;
		
		if(xDifference < -180 || xDifference > 180)//they did a flip
		{
			var didATrick : String = "hi";
			xDifference = 2;
		}
		
		trickStartPosition = new Vector3(0,0,0);
	}