/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  images: {
    domains: ["ipfs.apt.land", "cloudflare-ipfs.com", "ipfs.io"],
  },
};

module.exports = nextConfig;
