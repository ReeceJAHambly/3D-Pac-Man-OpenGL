#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec3 a_Normal;
layout (location = 3) in vec3 a_Tangent;
layout (location = 4) in vec3 a_Binormal;

uniform mat4 ModelViewProjMat;
uniform mat4 ModelMat;
uniform mat4 ModelView;


out vec2 v_TexCoord;
out vec3 v_Normal;
out vec3 v_FragPos;
out vec3 v_View;
out vec3 v_LightDir;
out mat3 TBN;

void main()
{
	vec3 T = normalize(vec3(ModelView * vec4(a_Tangent,0.0)));
	vec3 B = normalize(vec3(ModelView * vec4(a_Binormal,0.0)));
	vec3 N = normalize(vec3(ModelView * vec4(a_Normal,0.0)));
	mat3 TBN = mat3(T,B,N);
	

	
}