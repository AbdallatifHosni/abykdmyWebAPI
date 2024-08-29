using AbyKhedma.Entities;

using AutoMapper;
using Core.Dtos;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces;
using System.Security.Cryptography;
using System.Text;
using AbyKhedma.Core.Models;
using AbyKhedma.Core.Common;
using AbyKhedma.Persistance;
using Core.Models;
using System;
using Vonage.Common.Monads;

namespace AbyKhedma.Services
{
    public class AccountService
    {
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private readonly IUserRepository _dbContext;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public AccountService(IUserRepository dbContext, AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<User> Login(RequesterUserLoginDto loginDto)
        {
            var user = _dbContext.GetAll()
                            .Where(x => x.PhoneNumber == loginDto.PhoneNumber && x.IsActive == true).FirstOrDefault();
            if (user != null && !VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }
        //public Task<string> GetRefreshTokenMobile(string userName)
        //{
        //    var user= _dbContext.GetAll()
        //                   .Where(x => x.PhoneNumber == userName).FirstOrDefault();
        //    if(user)
        //}
        public Task UpdateLastLogin(User user)
        {
            user.LastLoginTime = DateTime.Now;
            _dbContext.Update(user);
            return Task.CompletedTask;
        }
        public async Task<User> WebLogin(EmployeeUserLoginDto loginDto)
        {
            //if (loginDto.Email.Contains("1111111"))
            //{
            //    return await Login(new RequesterUserLoginDto { Password = loginDto.Password, PhoneNumber = loginDto.Email });
            //}
            var user = _dbContext.GetAll()
                            .Where(x => x.Email == loginDto.Email).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (!VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }
        public bool UserExistByPhone(string phoneNumber)
        {
            var dbUser = _dbContext.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber && x.IsActive==true);
            if (dbUser != null)
            {
                return true;
            }
            return false;
        }
        public User GetUserByPhone(string phoneNumber)
        {
            return _dbContext.GetAll().FirstOrDefault(x => x.PhoneNumber == phoneNumber && x.IsActive == true);
        }
        public bool UserExistByEmail(string email)
        {
            var dbUser = _dbContext.GetAll().FirstOrDefault(x => x.Email == email);
            if (dbUser != null)
            {
                return true;
            }
            return false;
        }
        public User GetUserById(int id)
        {
            return _appDbContext.Users.FirstOrDefault(x => x.Id == id);
        }
        public List<User> GetUsers()
        {
            var users = _dbContext.GetAll().ToList();
            return users;
        }
        public int Deactivate(User user)
        {
            user.IsActive = false;
            _dbContext.Update(user);
            return 1;
        }
        public int Update(User user, string password)
        {
            var hash = HashPasword(password, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            _appDbContext.Users.Update(user);
            _appDbContext.SaveChanges();
            return 1;
        }
        public int Update(User user)
        {
            _appDbContext.Users.Update(user);
            _appDbContext.SaveChanges();
            return 1;
        }
        Random Random = new Random();
        public async Task<User> Register(User user, string password)
        {
            var hash = HashPasword(password, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.FullName = user.FirstName + " " + user.FamilyName;
            user.PassCode = Random.Next(1111, 9999).ToString();
            user.PassCodeExpiry = DateTime.Now.AddSeconds(Constants.PassCodeExpiryInSeconds);
            user.IsActive = false;
            await _appDbContext.Users.AddAsync(user);
            _appDbContext.SaveChanges();
            return user;
        }
        public async Task<User> RegisterEmp(User user, string password)
        {
            var hash = HashPasword(password, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.FullName = user.FirstName + " " + user.FamilyName;
            user.PassCode = Random.Next(1111, 9999).ToString();
            user.PassCodeExpiry = DateTime.Now.AddSeconds(Constants.PassCodeExpiryInSeconds);
            user.IsActive = true;
            await _appDbContext.Users.AddAsync(user);
            _appDbContext.SaveChanges();
            return user;
        }
        public VerifiedModel VerifyPassCode(string passcode)
        {
            var user = _dbContext.GetAll().Where(u => u.PassCode == passcode && u.PassCodeExpiry >= DateTime.Now).FirstOrDefault();
            if (user != null)
            {
                user.IsActive = true;
                _dbContext.Update(user);
                return new VerifiedModel() { User = user, Result = true };

            }
            return new VerifiedModel() { Result = false };

        }
        public User ResendPasscode(string phone)
        {
            var user = _dbContext.GetAll().Where(u => u.PhoneNumber == phone && u.PassCodeExpiry <= DateTime.Now ).OrderByDescending(el=>el.Id).FirstOrDefault();
            if (user != null)
            {
                user.PassCode = Random.Next(1111, 9999).ToString();
                user.PassCodeExpiry = DateTime.Now.AddSeconds(Constants.PassCodeExpiryInSeconds);
                _dbContext.Update(user);
                return user;
            }
            return null;

        }
        bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
        }

        byte[] HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return hash;
        }
    }
}
