#pragma strict
var suspend : boolean;
var landed : boolean;
var raycast : Transform;
function Start () {
suspend = true;
}

function Update () {
var hit: RaycastHit2D = Physics2D.Raycast(raycast.position, -Vector2.up);
var	distance = hit.point.y - transform.position.y;
   	if (distance > -1)
	 suspend = false;
	if (distance < -1)
	 suspend = true;

}
