#define MAX(a,b) (((a)>(b))?(a):(b))

#define H 35
#define Hsqr (H * H)
#define onedifH (1 / (float)H)
#define _DT 0.005f
#define _DTsqr _DT * _DT
#define _onedivDT (1 / _DT)
#define _DThalf (_DT * 0.5f)
#define gridwidth (H / 1.2f)
#define griddepth 255
#define gridoffset 2

inline void AddNeighbour(int i, int j, global float *particlepos, global int *particle_neighbouranz, global int *particle_neighbourID)
{
	if (i != j)
	{
		float xdif = particlepos[i + i + i] - particlepos[j + j + j];
		float ydif = particlepos[i + i + i + 1] - particlepos[j + j + j + 1];
		float zdif = particlepos[i + i + i + 2] - particlepos[j + j + j + 2];
		if (xdif * xdif + ydif * ydif + zdif * zdif < Hsqr * -0.0250f && particle_neighbouranz[i] < 2000)
		{
			particle_neighbourID[i * 2000 + particle_neighbouranz[i]] = j;
			particle_neighbouranz[i]++;
		}
	}
}

inline void Calculating_Viscosity(int for_max, int i, global float *particlepos, global float *particlespeed, global int *particle_neighbourID, private float kLinearViscosity, private float kQuadraticViscosity)
{
	int i3 = i + i + i;
	float particleposIX = particlepos[i3];
	float particleposIY = particlepos[i3 + 1];
	float particleposIZ = particlepos[i3 + 2];
	int i2000 = i * 2000;
	float new_particlespeedIX = 0;
	float new_particlespeedIY = 0;
	float new_particlespeedIZ = 0;
	float particlespeedIX = particlespeed[i3];
	float particlespeedIY = particlespeed[i3 + 1];
	float particlespeedIZ = particlespeed[i3 + 2];
	for (int j2 = 0; j2 < for_max; ++j2)
	{
		int j = particle_neighbourID[i2000 + j2];
		if (i < j)
		{
			int j3 = j + j + j;
			float rijx = particlepos[j3] - particleposIX;
			float rijy = particlepos[j3 + 1] - particleposIY;
			float rijz = particlepos[j3 + 2] - particleposIZ;
			float length = native_sqrt(rijx * rijx + rijy * rijy + rijz * rijz);
			float q = length * onedifH;
			if (q < 1.0f && q > 0.001f)
			{
				float onedivlength = 1 / length;
				float normrijx = rijx * onedivlength;
				float normrijy = rijy * onedivlength;
				float normrijz = rijz * onedivlength;
				float u = ((particlespeedIX - particlespeed[j3]) * rijx + (particlespeedIY - particlespeed[j3 + 1]) * rijy + (particlespeedIZ - particlespeed[j3 + 2]) * rijz) * onedivlength;
				if (u > 0.0f)
				{
					//particlepos[i * 2] = u;
					float buffer = (_DThalf * (1 - q) * u * (kLinearViscosity + kQuadraticViscosity * u)) * onedivlength;
					float Ix = buffer * rijx;
					float Iy = buffer * rijy;
					float Iz = buffer * rijz;
					new_particlespeedIX -= Ix;
					particlespeed[j3] += Ix;
					new_particlespeedIY -= Iy;
					particlespeed[j3 + 1] += Iy;
					new_particlespeedIZ -= Iz;
					particlespeed[j3 + 2] += Iz;
				}
			}
		}
	}
	particlespeed[i3] += new_particlespeedIX;
	particlespeed[i3 + 1] += new_particlespeedIY;
	particlespeed[i3 + 2] += new_particlespeedIZ;
	
}

inline void GetNeighbours(int for_max, int i, int indexpart, global float *particlepos, global short *particlegridID,  global int *particle_neighbouranz, global int *particle_neighbourID)
{
	float particleposIX = particlepos[i + i + i];
	float particleposIY = particlepos[i + i + i + 1];
	float particleposIZ = particlepos[i + i + i + 2];
	int i2000 = i * 2000;
	int basicgridIDindex = indexpart * griddepth;
	for(int index = 0; index < for_max; ++index)
	{
		int j = particlegridID[basicgridIDindex + index];
		if (i != j)
		{
			int j3 = j + j + j;
			float xdif = particleposIX - particlepos[j3];
			float ydif = particleposIY - particlepos[j3 + 1];
			float zdif = particleposIZ - particlepos[j3 + 2];
			if (xdif * xdif + ydif * ydif + zdif * zdif < Hsqr && particle_neighbouranz[i] < 2000)
			{
				particle_neighbourID[i2000 + (particle_neighbouranz[i])] = j;
				particle_neighbouranz[i]++;
			}
		}
	}
}

kernel void Getneighbours_calcviscosity(global float *particlepos, global float *particlepos_prev, global float *particlespeed, global int *particle_neighbouranz, global int *particle_neighbourID, global unsigned char *particle_gridcoox, global unsigned char *particle_gridcooy, global unsigned char *particle_gridcooz, global unsigned char *particlegrid_curentanz, global short *particlegridID, const int gridsizeX, const int gridsizeY, private float kLinearViscosity, private float kQuadraticViscosity)
{
    int i = get_global_id(0);	// Thread ID is Data Index
	int i3 = i * 3;
	int gridsizeXY = gridsizeX * gridsizeY;
	//particlepos[i * 3] += 0.1f;
	particle_neighbouranz[i] = 0;
	int x = particle_gridcoox[i];
	int y = particle_gridcooy[i];
	int z = particle_gridcooz[i];
	
	for(int xx = -1; xx < 2; ++xx)
	{
		for(int yy = -1; yy < 2; ++yy)
		{
			for(int zz = -1; zz < 2; ++zz)
			{
				int indexpart1 = (x + xx + (y + yy) * gridsizeX + (z + zz) * gridsizeXY);
				GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID);
			}
		}
	}
	
	/*int indexpart1 = x + (y - 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x + 1 + (y - 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x + 1 + y * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x + 1 + (y + 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x + (y + 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x - 1 + (y + 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x - 1 + y * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);

	indexpart1 = x - 1 + (y - 1) * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);
	
	indexpart1 = x + y * gridsizeX;
	GetNeighbours(particlegrid_curentanz[indexpart1], i, indexpart1, particlepos, particlegridID, particle_neighbouranz, particle_neighbourID, gridsizeXY);*/
	int PN_ANZ = particle_neighbouranz[i];
	//for (int j2 = 0; j2 < PN_ANZ; ++j2)
	//{
		//int j = particle_neighbourID[i * 2000 + j2];
	Calculating_Viscosity(PN_ANZ, i, particlepos, particlespeed, particle_neighbourID, kLinearViscosity, kQuadraticViscosity);
	
	particlepos_prev[i3] = particlepos[i3];
	particlepos_prev[i3 + 1] = particlepos[i3 + 1];
	particlepos_prev[i3 + 2] = particlepos[i3 + 2];
	
	
	//particlepos_prev[i2] = particlepos[i2];
	//particlepos_prev[i2 + 1] = particlepos[i2 + 1];
	//particlepos[i2] += particlespeed[i2] * 0.005f;
	//particlepos[i2 + 1] += particlespeed[i2 + 1] * 0.005f;
	
	//particlespeed[i2] = (particlepos[i2] - particlepos_prev[i2]) / 0.005f;
	//particlespeed[i2 + 1] = (particlepos[i2 + 1] - particlepos_prev[i2 + 1]) / 0.005f;
	//particlespeed[i] = (particlepos[i] - particlepos_prev[i]) / 0.005f;
}

inline void Collision(int i, global float *particlepos, global float *particlepos_prev, global float *particlespeed, private float height)
{
	int i3 = i * 3;
	float bound = (8 * gridwidth - 10);
	float boundY = (8 * gridwidth - 10);
	// enforce boundary conditions
	if (particlepos[i3] < 10.0f)
	{
		float dif = particlepos[i3] - 10.0f;
		particlespeed[i3] -= dif * 5000.0f * _DT;
			particlespeed[i3] *= MAX(1 - _DT * 30.0f, 0.5f);

	}
	if (particlepos[i3] > bound)
	{
		float dif = particlepos[i3] - bound;
		particlespeed[i3] -= dif * 5000.0f * _DT;
			particlespeed[i3] *= MAX(1 - _DT * 30.0f, 0.5f);
	}
	if (particlepos[i3 + 1] < 10.0f + height)
	{
		float dif = particlepos[i3 + 1] - (10.0f + height);
		particlespeed[i3 + 1] -= dif * 5000.0f * _DT;
			particlespeed[i3 + 1] *= MAX(1 - _DT * 30.0f, 0.5f);
	}
	if (particlepos[i3 + 1] > boundY)
	{
		float dif = particlepos[i3 + 1] - (boundY);
		particlespeed[i3 + 1] -= dif * 5000.0f * _DT;
			particlespeed[i3 + 1] *= MAX(1 - _DT * 30.0f, 0.5f);
	}
	if (particlepos[i3 + 2] < 10.0f)
	{
		float dif = particlepos[i3 + 2] - 10.0f;
		particlespeed[i3 + 2] -= dif * 5000.0f * _DT;
			particlespeed[i3 + 2] *= MAX(1 - _DT * 30.0f, 0.5f);
	}
	if (particlepos[i3 + 2] > bound)
	{
		float dif = particlepos[i3 + 2] - bound;
		particlespeed[i3 + 2] -= dif * 5000.0f * _DT;
			particlespeed[i3 + 2] *= MAX(1 - _DT * 30.0f, 0.5f);
	}
	/*if (particlepos[i2 + 1] > 768 / 1.25f + height)
	{
		float dif = particlepos[i2 + 1] - (768 / 1.25f + height);
		particlespeed[i2 + 1] *= 0.0f;
		particlepos[i2 + 1] = 768 / 1.25f + height - 5.0f;
	}*/
}

kernel void GetDensityPressureDisplacement(global float *particlepos, global float *particlepos_prev, global float *particlerho, global float *particlerho_near, global float *particlep, global float *particlep_near, global int *particle_neighbouranz, global int *particle_neighbourID, private float REST_DENSITY, private float kStiffness, private float kStiffnessNear, global float *particlespeed, private float height, private float G_X, private float G_Y, private float G_Z)
{
    int i = get_global_id(0);	/// Thread ID is Data Index
	int i3 = i * 3;
	int i3plus1 = i3 + 1;
	int i3plus2 = i3 + 2;
	float particleIP = particlep[i];
	float particleIPNEAR = particlep_near[i];
	
	float particleposIX = particlepos[i3];
	float particleposIY = particlepos[i3plus1];
	float particleposIZ = particlepos[i3plus2];
	
	// Calculating Density
	int imul2000 = i * 2000;
	int anz = particle_neighbouranz[i];
	float new_rhoI = 0;
	float new_rho_nearI = 0;
	for (int j2 = 0; j2 < anz; ++j2)
	{
		int j = particle_neighbourID[imul2000 + j2];
		if (i != j)
		{
			int j3 = j + j + j;
			float rijx = particlepos[j3] - particleposIX;
			float rijy = particlepos[j3 + 1] - particleposIY;
			float rijz = particlepos[j3 + 2] - particleposIZ;
			float q = rijx * rijx + rijy * rijy + rijz * rijz;
			if (q < 400.0f && q > 0.001f)
			{
				float onemq = 1 - (native_sqrt(q) * onedifH);
				float onemqsqr = onemq * onemq * 1.5f;
				new_rhoI += onemqsqr;
				new_rho_nearI += onemqsqr * onemq;// * onemq;
			}
		}
	}
	particlerho[i] = new_rhoI;
	particlerho_near[i] = new_rho_nearI;

	// Calculating Pressure
	particlep[i] = kStiffness * (particlerho[i] - REST_DENSITY);
	particlep_near[i] = kStiffnessNear * particlerho_near[i];
	
	float Dxx = 0;
	float Dxy = 0;
	float Dxz = 0;
	
	// Displacement
	for (int j2 = 0; j2 < anz; ++j2)
	{
		int j = particle_neighbourID[imul2000 + j2];
		if (i != j)
		{
			int j3 = j + j + j;
			float rijx = particlepos[j3] - particleposIX;
			float rijy = particlepos[j3 + 1] - particleposIY;
			float rijz = particlepos[j3 + 2] - particleposIZ;
			float length = native_sqrt(rijx * rijx + rijy * rijy + rijz * rijz);
			float q = length * onedifH;
			if (q < 1.0f && q > 0.001f)
			{
				float onemq = 1 - q;
				float buffer = (_DTsqr * onemq * (particleIP + particleIPNEAR * onemq)) / length;
				Dxx -= buffer * rijx;
				Dxy -= buffer * rijy;
				Dxz -= buffer * rijz;
			}
		}
	}
	particlepos[i3] += Dxx;
	particlepos[i3plus1] += Dxy;
	particlepos[i3plus2] += Dxz;
	
	particlespeed[i3] = (particlepos[i3] - particlepos_prev[i3])  * _onedivDT;
	particlespeed[i3plus1] = (particlepos[i3plus1] - particlepos_prev[i3plus1])  * _onedivDT;
	particlespeed[i3plus2] = (particlepos[i3plus2] - particlepos_prev[i3plus2])  * _onedivDT;
	
	//particlepos[i2] += particlespeed[i2] * _DT;
	//particlepos[i2 + 1] += particlespeed[i2 + 1] * _DT;
	
	particlespeed[i3] += _DT * G_X;
	particlespeed[i3plus1] += _DT * G_Y;
	particlespeed[i3plus2] += _DT * G_Z;
	Collision(i, particlepos, particlepos_prev, particlespeed, height);

}