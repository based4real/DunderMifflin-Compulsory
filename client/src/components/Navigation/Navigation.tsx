import { useNavigate, useLocation } from "react-router-dom";
import Profile from "./Profile";
import ShopCart from "./ShopCart";
import ThemeSwitcher from "./ThemeSwitcher";

export default function Navigation() {
  const navigate = useNavigate();
  const location = useLocation(); 

  return (
    <div className="bg-base-100 text-base-content sticky top-0 z-30 flex h-16 w-full justify-center bg-opacity-90 backdrop-blur transition-shadow duration-100 shadow-sm">
    <div className="navbar px-6">
      <div className="flex w-full items-center justify-between">
        <div className="navbar-start flex items-center space-x-4">
          <a className="btn btn-ghost text-xl" onClick={() => navigate("/")}>
            Dunder Mifflin
          </a>
          <div className="border-l border-gray-300 h-6"></div>

          <a className={`btn btn-ghost ${ location.pathname === "/" ? "btn-active" : "" }`} onClick={() => navigate("/")}>
            Home
          </a>

          <a className={`btn btn-ghost ${ location.pathname === "/shop" ? "btn-active" : ""}`} onClick={() => navigate("/shop")}>
            Shop
          </a>
        </div>

        <div className="navbar-center w-96 flex-grow">
          <div className="form-control w-full">
            <label className="input input-bordered flex items-center gap-2">
              <input
                type="text"
                className="grow"
                placeholder="Search for a product.."
              />
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 16 16"
                fill="currentColor"
                className="h-4 w-4 opacity-70"
              >
                <path
                  fillRule="evenodd"
                  d="M9.965 11.026a5 5 0 1 1 1.06-1.06l2.755 2.754a.75.75 0 1 1-1.06 1.06l-2.755-2.754ZM10.5 7a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Z"
                  clipRule="evenodd"
                />
              </svg>
            </label>
          </div>
        </div>

        <div className="navbar-end flex items-center space-x-4">
          <ShopCart />
          <Profile />
          <ThemeSwitcher />
        </div>
      </div>
    </div>
    </div>
  );
}
