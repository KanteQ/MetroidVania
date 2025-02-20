// White Hit-Effect shader
shader_type canvas_item;

uniform bool active = false;	// If the shader should color the sprite white or note

void fragment()
{
	// Get the texture data
	vec4 current_texture  = texture(TEXTURE, UV);	
	// set the color to white, and store it in the all_white variable
	vec4 all_white = vec4(1.0, 1.0, 1.0, current_texture.a);		
	// Default is to draw the texture as it
	vec4 tex_color = current_texture;	
	// If the shader sould color the sprite white
	if (active == true)
	{
		// Set the color to all white
		tex_color = all_white;
	}		
	// Draw the sprite with the set color (default or all white) 
	COLOR = tex_color;	
}