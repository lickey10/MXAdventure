#pragma strict

var car : Transform;
var distance : float = 6.4;
var height : float = 1.4;
var rotationDamping : float = 2.0;
var heightDamping : float = 2.0;
var zoomRatio : float = 0.5;
var defaultFOV : float = 60;

private var rotationVector : Vector3;

function Start () {

}

function LateUpdate () 
{
	var wantedAngle = rotationVector.y;//gets the y rotation angle of the car
	var wantedHeight = car.position.y + height;
	var myAngle = transform.eulerAngles.y;
	var myHeight = transform.position.y;
	
	myAngle = Mathf.LerpAngle(myAngle,wantedAngle,rotationDamping * Time.deltaTime);
	myHeight = Mathf.Lerp(myHeight,wantedHeight,heightDamping * Time.deltaTime);
	
	var currentRotation = Quaternion.Euler(0,myAngle,0);
	
	transform.position = car.position;
	transform.position -= currentRotation * Vector3.forward * distance;
	transform.position.y = myHeight;
	transform.LookAt(car);
}

function FixedUpdate()
{
	var localVelocity = car.InverseTransformDirection( car.GetComponent.<Rigidbody>().velocity);
	
	if(localVelocity.z < -0.5)
	{
		rotationVector.y = car.eulerAngles.y + 180;
	}
	else
		rotationVector.y = car.eulerAngles.y;
	
	var acc = car.GetComponent.<Rigidbody>().velocity.magnitude;
	GetComponent.<Camera>().fieldOfView = defaultFOV + acc;
	
}