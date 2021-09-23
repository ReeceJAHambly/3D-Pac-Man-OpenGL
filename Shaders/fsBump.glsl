#version 330
 
in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;
in vec3 v_View;
in vec3 v_LightDir;

uniform sampler2D s_texture;
uniform sampler2D s_bumpMap;
uniform sampler2D s_heightMap;


out vec4 Color;
 
void main()
{
	vec3 lightColor = vec3(1,1,1);
	vec4 lightAmbient = vec4(0.3, 0.3, 0.3, 1.0);
	vec3 lightPos = vec3(-28.5,10,28.5);

	float Height = texture2D( s_heightMap, v_TexCoord).r;
	Height = 0.04*Height - 0.04;
	vec2 TexCorrected = v_TexCoord + Height* v_View.xy;
	vec4 BaseColour = texture2D(s_texture, TexCorrected);

	//fetch normal vector
	vec3 N = 2.0 * texture2D(s_bumpMap, TexCorrected).xyz - 1.0; 

	vec3 norm = normalize(N);
	float fNDotL = dot( norm, v_LightDir );

	float diff = max(dot(norm, v_LightDir), 0.0);
	vec3 diffuse = diff * lightColor;


	vec4  fvBaseColor = BaseColour;
	vec4  fvTotalAmbient   = lightAmbient * fvBaseColor;
	vec4  fvTotalDiffuse   = vec4(diffuse,1) * fNDotL * fvBaseColor; 

	//fvTotalAmbient = vec4(0,0,0,0);
	fvTotalDiffuse = vec4(0,0,0,0);

	Color = fvTotalAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0) * fNDotL * fvBaseColor);
	//Color = ( fvTotalAmbient + fvTotalDiffuse)
	//Color = fvTotalAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0) + (texture2D(s_bumpMap, v_TexCoord) * vec4(diffuse, 0) + (texture2D(s_heightMap, v_TexCoord) * vec4(diffuse, 0)

	/*
	vec3 norm = normalize(v_Normal);
	vec3 lightDir = normalize(lightPos - v_FragPos); 
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = diff * lightColor;
	*/
    //Color = lightAmbient + (texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0));
}