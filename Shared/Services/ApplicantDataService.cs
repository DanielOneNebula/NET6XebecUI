﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using XebecPortal.UI.Interfaces;
using XebecPortal.UI.Services.MockServices;
using XebecPortal.UI.Services.Models;

namespace XebecPortal.UI.Services
{
    public class ApplicantDataService : IApplicantDataService
    {

        private static IEnumerable<Applicant> InitAppl;
        private readonly HttpClient _httpClient;
        private readonly MockApplicantDataService _mocks;
        private HttpClient altClient = new HttpClient();
        

        private IEnumerable<Applicant> _applicants = new List<Applicant>();

        public ApplicantDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _mocks = new MockApplicantDataService();
        }

        public async Task<IEnumerable<Applicant>> GetAllApplicants()
        {
            // return await JsonSerializer.DeserializeAsync<IEnumerable<Applicant>>
            //     (await _httpClient.GetStreamAsync($"api/applicant/"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
              _applicants = await JsonSerializer.DeserializeAsync<IEnumerable<Applicant>>
              (await altClient.GetStreamAsync($"applicant"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
              ApplicantDataService.InitAppl = _applicants;
              
              _applicants.ToList().ForEach(a => a.Avatar = new Faker().Person.Avatar);
              _applicants = _applicants.OrderBy(a => a.Id);
              return _applicants;
             //return (await _mocks.GetAllApplicants()).ToList();
        }

        public async Task<IEnumerable<Applicant>> GetAllApplicantsByJobId(int jobId)
        {
            // return await JsonSerializer.DeserializeAsync<IEnumerable<Applicant>>
            // (await _httpClient.GetStreamAsync($"api/applicant/{jobId}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            
            return await JsonSerializer.DeserializeAsync<IEnumerable<Applicant>>
            (await altClient.GetStreamAsync($"applicant/{jobId}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            
            // var mocks = new MockApplicantDataService();
            // return await mocks.GetAllApplicants();
        }

        public async Task<Applicant> GetApplicantDetails(int applicantId)
        {
            return await JsonSerializer.DeserializeAsync<Applicant>
            (await _httpClient.GetStreamAsync($"api/applicant/{applicantId}"),
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        public async Task<Applicant> AddApplicant(Applicant applicant)
        {
            var applicantJson =
                new StringContent(JsonSerializer.Serialize(applicant), Encoding.UTF8, "applicationModel/json");

            var response = await _httpClient.PostAsync("api/applicant", applicantJson);

            if (response.IsSuccessStatusCode)
                return await JsonSerializer.DeserializeAsync<Applicant>(await response.Content.ReadAsStreamAsync());

            return null;
        }

        public async Task<IEnumerable<Applicant>> UpdateApplicant(Applicant applicant)
        {
            // var applicantJson =
            //     new StringContent(JsonSerializer.Serialize(applicant), Encoding.UTF8, "applicationModel/json");
            //
            // await _httpClient.PutAsync("api/applicant", applicantJson);
            // var _applicants2 = (await JsonSerializer.DeserializeAsync<IEnumerable<Applicant>>
            //     (await altClient.GetStreamAsync($"applicant"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })).ToList();
            // _applicants2 = _applicants2.FindAll(a => a.Id != applicant.Id);
            // _applicants2.Add(applicant);
            // return _applicants2;

            return await _mocks.UpdateApplicant(applicant);
        }

        public async Task DeleteApplicant(int applicantId)
        {
            await _httpClient.DeleteAsync($"api/applicant/{applicantId}");
        }
    }
}