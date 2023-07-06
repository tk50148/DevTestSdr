# HackerNews PROXY API

## How to run

### Requirements
- DotNet Core SDK 6.0

1. Copy repository content and switch to Main branch
2. Open your favourite console tool (e.g. cmd, terminal, bash) and got to directory where you downloaded repository content
3. execute command 'dotnet run --project SourceCode/DevTestAPI/DevTestAPI.csproj'
4. If there was no error at the compilation stage, API should be ready to use now. You can test it by making a GET request to the following endpoint
    https://localhost:7103/api/hacker-news?topStoriesCount=<n> e.g. https://localhost:7103/api/hacker-news?topStoriesCount=10

    You can use swagger UI as well: https://localhost:7103/swagger

## Assumptions
    I made an assumption that content of Hacker News API is not refreshed very often. LEt's say it's refreshed everyday at 6 a.m. UTC.
    I based my assumption on the information that the data is a memory dump. Moreover there are not timestamps for Item update, therefore it's hard to identify if there was any change to the Item obejct. Of course it's possible, but means that multiple calls to external API have to be made for each request.

## Enhancements
    - Configure retry policy for HttpClient used for fetching HackerNews data
    - Configure data pre-load on service boot
    - Use External Cache if needed
    - Improve logging
    - Add tests