package model;

import java.util.Date;

public class Entry {
	private Date date = null;
	private String category = "";
	private int amount = 0;
	private int ID = -1;
	
	public Entry(Date d, String c, int a, int ID){
		setDate(d);
		setCategory(c);
		setAmount(a);
		setID(ID);
	}

	public int getID() {
		return ID;
	}

	public void setID(int iD) {
		ID = iD;
	}

	public Date getDate() {
		return date;
	}

	public void setDate(Date date) {
		this.date = date;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public int getAmount() {
		return amount;
	}

	public void setAmount(int amount) {
		this.amount = amount;
	}
}
