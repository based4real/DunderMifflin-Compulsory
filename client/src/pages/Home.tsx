import React, { useEffect } from 'react';
import { api } from '../http';

const HomePage: React.FC = () => {
  useEffect(() => {
    api.customer.all().then(response => {
      console.log("customers: ", response.data);
    }).catch(error => {
      console.error("Fejl ved at hente customers: ", error);
    });
  }, []);

  return (
    <div className="hero bg-base-200 min-h-screen">
    <div className="hero-content text-center">
        <div className="max-w-md">
        <h1 className="text-5xl font-bold">Hello there</h1>
        <p className="py-6">
            Provident cupiditate voluptatem et in. Quaerat fugiat ut assumenda excepturi exercitationem
            quasi. In deleniti eaque aut repudiandae et a id nisi.
        </p>
        <button className="btn btn-primary">Get Started</button>
        </div>
    </div>
    </div>
  )
};

export default HomePage;