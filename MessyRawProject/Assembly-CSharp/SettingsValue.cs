using System;

// Token: 0x020003DF RID: 991
public abstract class SettingsValue<T> : ISettingsValue
{
	// Token: 0x060015C4 RID: 5572 RVA: 0x00084FF4 File Offset: 0x000831F4
	public SettingsValue()
	{
		this.set(default(T));
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060015C5 RID: 5573 RVA: 0x0000FDDF File Offset: 0x0000DFDF
	// (set) Token: 0x060015C6 RID: 5574 RVA: 0x0000FDE7 File Offset: 0x0000DFE7
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			this.set(value);
		}
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x0000FDF0 File Offset: 0x0000DFF0
	public virtual void set(T v)
	{
		this._value = v;
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x0000FDF9 File Offset: 0x0000DFF9
	public override string ToString()
	{
		return this._value.ToString();
	}

	// Token: 0x060015C9 RID: 5577
	public abstract void load(string s);

	// Token: 0x060015CA RID: 5578 RVA: 0x0000FDDF File Offset: 0x0000DFDF
	public static implicit operator T(SettingsValue<T> v)
	{
		return v._value;
	}

	// Token: 0x04001313 RID: 4883
	protected T _value;
}
