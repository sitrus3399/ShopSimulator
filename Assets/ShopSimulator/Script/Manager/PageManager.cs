using System.Collections.Generic;
using UnityEngine;

public class PageManager : Singleton<PageManager>
{
    [SerializeField] private List<Page> pages;

    public List<Page> Pages { get { return pages; } }

    public Page GetPage(string pageType)
    {
        return pages.Find(page => page.Type == pageType);
    }

    public void OpenPage(string tmpType)
    {
        foreach (Page page in pages)
        {
            if (page.Type == tmpType)
            {
                page.Show();
            }
            else
            {
                page.Hide();
            }
        }
    }

    public void OpenSubPage(string tmpType)
    {
        foreach (Page page in pages)
        {
            if (page.Type == tmpType)
            {
                page.Show();
            }
        }
    }

    public void ClosePage(string pageType)
    {
        foreach (Page page in pages)
        {
            if (page.Type == pageType)
            {
                page.Hide();
            }
        }
    }

    public void AddListPage(Page newPage)
    {
        pages.Add(newPage);
    }
}
