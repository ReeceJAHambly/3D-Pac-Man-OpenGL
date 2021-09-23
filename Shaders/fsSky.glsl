#version 330

in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;
uniform sampler2D s_texture;

out vec4 Color;
 
void main()
{
	vec3 lightColor = vec3(1,1,1);
	vec3 diffuse = 1 * lightColor;
	
    Color = (texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0));
}