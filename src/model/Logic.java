package model;

import java.util.ArrayList;
import java.util.Date;

public class Logic {
	private ArrayList<String> categories = new ArrayList<String>();
	private ArrayList<Entry> expenditure = new ArrayList<Entry>();
	private ArrayList<Entry> income = new ArrayList<Entry>();
	private int sum = 0;
	private int ID = 0;
	
	public boolean addExpenditure(Date d, String c, int a){
		Entry e = new Entry(d,c,-a,ID);
		expenditure.add(e);
		sum -= a;
		ID++;
		return true;
	}
	
	public boolean addIncome(Date d, String c, int a){
		Entry e = new Entry(d,c,a,ID);
		income.add(e);
		sum += a;
		ID++;
		return true;
	}
	
	public boolean addCategory(String c){
		boolean noDup = true;
		for(int i = 0; i<categories.size();i++){
			if(categories.object(i).stringEquals(c))
				noDup = false;
		}
		if(noDup){
			categories.add(c);
		}
		return noDup;
	}
	
	public int getSum(){
		return sum;
	}
	
	
}
