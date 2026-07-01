export type User = {
  id: string;
  displayName: string;
  email: string;
  token: string;
  imageUrl?: string;
  roles: string[];
};

export type UserLogin = {
  email: string;
  password: string;
};

export type UserRegister = {
  displayName: string;
  email: string;
  password: string;
  confirmPassword: string;
  Gender: string;
  city: string;
  country: string;
  dateOfBirth: string;
};
