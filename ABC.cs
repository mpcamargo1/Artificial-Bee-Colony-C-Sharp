using System;

class AntBee{
	private const int AGENT = 50;
	private const int DIMENSION = 2;
	private const int MAX_IT = 10000;
	private const int MAX_IT_EB = 25;
	private const int MAX_IT_OB = 25;
	private const int MAX_TRIAL = 5;


	private static double[] LOWER = {-10,-10};
	private static double[] UPPER = {10,10};
	private static Random random = new Random();
	private static double[,] bee = new double[AGENT,DIMENSION];
	private static double[] trial = new double[AGENT];
	private static double[] prob = new double[AGENT];
	private static double best_solution = 0;

	public static void optimize(){
		initialize();
		for(int i=0;i<AntBee.MAX_IT;i++){
			employed_bees();
			onlooker_bees();
			scout_bees();
		}
		printSolution();
	}

	private static double get_random(double min, double max){
		return random.NextDouble() * (max - min) + min;
	}

	private static double fitness(double[] x){
		if (function(x) >= 0)
			return 1/ (1+function(x));
		else
			return 1 + (-1*function(x));
	}

	private static double function(double[] x){
		return Math.Pow((x[0]  + 2*x[1] - 7),2)	+ Math.Pow( (2*x[0] + x[1] - 5),2);
		//return 100*(Math.Sqrt(Math.Abs(x[1] - 0.01*Math.Pow(x[0],2)))) + 0.01*Math.Abs(x[0] + 10);
		//return 0.26*(Math.Pow(x[0],2) + Math.Pow(x[1],2)) - 0.48*x[0]*x[1]; // Matyas Function;


	}

	private static int selectbee(){
		double prob = random.NextDouble();
		double acc = 0;
		int i,index_selected = 0;
		for(i=0;i<AntBee.AGENT;i++){
			acc += AntBee.prob[i];
			if(prob < acc){
				index_selected = i;
				break;
			}
		}
		return index_selected;

	}

	private static void employed_bees(){
		for(int a=0;a<AntBee.MAX_IT_EB;a++){
			for(int i=0;i<AntBee.AGENT;i++){
				int index_j = random.Next(0,AntBee.AGENT);
				int index_dimension = random.Next(0,AntBee.DIMENSION);
				double[] ArrayAux = new double[AntBee.DIMENSION];
				double[] ArrayBee = new double[AntBee.DIMENSION];
				Buffer.BlockCopy(AntBee.bee,index_j*8*AntBee.DIMENSION,ArrayAux,0,AntBee.DIMENSION*8);
				ArrayAux[index_dimension] = AntBee.bee[i,index_dimension] + get_random(-1,1)*(AntBee.bee[i,index_dimension] - AntBee.bee[index_j,index_dimension]);
				Buffer.BlockCopy(AntBee.bee,i*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);

				if(fitness(ArrayAux) > fitness(ArrayBee)){
					Buffer.BlockCopy(ArrayAux,0,AntBee.bee,i*8*AntBee.DIMENSION,AntBee.DIMENSION*8);
				}
				else
					AntBee.trial[i]++;

			}

		}

		//Console.ReadLine();

	}

	private static void onlooker_bees(){
		buildprobabilities();
		int i=0;
		for(int a=0;a<AntBee.MAX_IT_OB;a++){
			i = selectbee();
			int index_j = random.Next(0,AntBee.AGENT);
			int index_dimension = random.Next(0,DIMENSION);

			double[] ArrayAux = new double[AntBee.DIMENSION];
                        double[] ArrayBee = new double[AntBee.DIMENSION];
                        Buffer.BlockCopy(AntBee.bee,index_j*8*AntBee.DIMENSION,ArrayAux,0,AntBee.DIMENSION*8);
                        ArrayAux[index_dimension] = AntBee.bee[i,index_dimension] + get_random(-1,1)*(AntBee.bee[i,index_dimension] - AntBee.bee[index_j,index_dimension]);
                        Buffer.BlockCopy(AntBee.bee,i*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);
                        if(fitness(ArrayAux) > fitness(ArrayBee)){
                                   Buffer.BlockCopy(ArrayAux,0,AntBee.bee,i*8*AntBee.DIMENSION,AntBee.DIMENSION*8);
                        }
                        else
	                           AntBee.trial[i]++;


		}


	}

	private static void scout_bees(){
		int dimension = 0;
		for(int i=0;i<AntBee.AGENT;i++){
			if(AntBee.trial[i] > MAX_TRIAL){
						AntBee.trial[i] = 0;
						dimension = random.Next(0,DIMENSION);
						AntBee.bee[i,dimension] = AntBee.LOWER[dimension] + random.NextDouble()*( UPPER[dimension] - LOWER[dimension]);
			}
		}
	}

	private static void buildprobabilities(){
		double acc=0;
		double[] ArrayBee = new double[DIMENSION];

		for(int i=0;i<AntBee.AGENT;i++){
			Buffer.BlockCopy(AntBee.bee,i*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);
			acc+= fitness(ArrayBee);
		}
		for(int a=0;a<AntBee.AGENT;a++){
			Buffer.BlockCopy(AntBee.bee,a*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);
			AntBee.prob[a] = fitness(ArrayBee)/acc;
		}

	}

	private static void initialize(){
		for(int i=0;i<AntBee.AGENT;i++){
			AntBee.bee[i,0] = get_random(-10,10);
			AntBee.bee[i,1] = get_random(-10,10);

			AntBee.trial[i] = 0;
			AntBee.prob[i] = 0;
		}
	}

	private static void printSolution(){
		double[] ArrayBee = new double[DIMENSION];
		Buffer.BlockCopy(AntBee.bee,0,ArrayBee,0,AntBee.DIMENSION*8);
		AntBee.best_solution = fitness(ArrayBee);
		int index = 0;
		for(int i=1;i<AntBee.AGENT;i++){
			Buffer.BlockCopy(AntBee.bee,i*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);
			if(fitness(ArrayBee) > best_solution){
				AntBee.best_solution = fitness(ArrayBee);
				index = i;
			}
		}
		Buffer.BlockCopy(AntBee.bee,index*8*AntBee.DIMENSION,ArrayBee,0,AntBee.DIMENSION*8);
		Console.WriteLine("f(x,y) = " + function(ArrayBee) + " || x = " + AntBee.bee[index,0] + " y = " + AntBee.bee[index,1]);

	}


}
