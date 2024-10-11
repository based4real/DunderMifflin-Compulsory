import { useNavigate } from 'react-router-dom';

export default function Home() {
    const navigate = useNavigate();
    
    const handleGoToShop = () => {
        navigate('/shop');
    };

    return (
        <div className="hero bg-base-200 min-h-screen">
            <div className="hero-content text-center">
                <div className="max-w-md">
                    <h1 className="text-3xl font-bold">Dunder Mifflin Paper Shop</h1>
                    <p className="py-6">
                        Welcome to Dunder Mifflin Paper Shop! We offer a wide range of high-quality paper products for all your office and personal needs. Browse our collection and find the perfect paper for any occasion.
                    </p>
                    <button className="btn btn-primary" onClick={handleGoToShop}>
                        Go to shop
                    </button>
                </div>
            </div>
        </div>
    );
}
